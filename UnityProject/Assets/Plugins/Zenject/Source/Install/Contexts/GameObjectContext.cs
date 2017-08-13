#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject.Internal;

#pragma warning disable 649

namespace Zenject
{
    public class GameObjectContext : RunnableContext
    {
        readonly List<object> _dependencyRoots = new List<object>();

        [SerializeField]
        [Tooltip("Note that this field is optional and can be ignored in most cases.  This is really only needed if you want to control the 'Script Execution Order' of your subcontainer.  In this case, define a new class that derives from MonoKernel, add it to this game object, then drag it into this field.  Then you can set a value for 'Script Execution Order' for this new class and this will control when all ITickable/IInitializable classes bound within this subcontainer get called.")]
        [FormerlySerializedAs("_facade")]
        MonoKernel _kernel;

        DiContainer _container;

        public override DiContainer Container
        {
            get { return _container; }
        }

        public override IEnumerable<GameObject> GetRootGameObjects()
        {
            return new[] { this.gameObject };
        }

        [Inject]
        public void Construct(
            DiContainer parentContainer)
        {
            Assert.IsNull(_container);

            _container = parentContainer.CreateSubContainer();

            Initialize();
        }

        protected override void RunInternal()
        {
            foreach (var instance in GetInjectableMonoBehaviours().Cast<object>())
            {
                if (instance is MonoKernel)
                {
                    Assert.That(ReferenceEquals(instance, _kernel),
                        "Found MonoKernel derived class that is not hooked up to GameObjectContext.  If you use MonoKernel, you must indicate this to GameObjectContext by dragging and dropping it to the Kernel field in the inspector");
                }

                _container.QueueForInject(instance);
            }

            _container.IsInstalling = true;

            try
            {
                InstallBindings();
            }
            finally
            {
                _container.IsInstalling = false;
            }

            Log.Debug("GameObjectContext: Resolving all dependencies...");

            Assert.That(_dependencyRoots.IsEmpty());
            _dependencyRoots.AddRange(_container.ResolveDependencyRoots());

            _container.FlushInjectQueue();

            if (_container.IsValidating)
            {
                // The root-level Container has its ValidateValidatables method
                // called explicitly - however, this is not so for sub-containers
                // so call it here instead
                _container.ValidateValidatables();
            }

            Log.Debug("GameObjectContext: Initialized successfully");

            // Normally, the IInitializable.Initialize method would be called during MonoKernel.Start
            // However, this behaviour is undesirable for dynamically created objects, since Unity
            // has the strange behaviour of waiting until the end of the frame to call Start() on
            // dynamically created objects, which means that any GameObjectContext that is created
            // dynamically via a factory cannot be used immediately after calling Create(), since
            // it will not have been initialized
            // So we have chosen to diverge from Unity behaviour here and trigger IInitializable.Initialize
            // immediately - but only when the GameObjectContext is created dynamically.  For any
            // GameObjectContext's that are placed in the scene, we still want to execute
            // IInitializable.Initialize during Start()
            if (gameObject.scene.isLoaded && !_container.IsValidating)
            {
                _kernel = _container.Resolve<MonoKernel>();
                _kernel.ForceInitialize();
            }
        }

        protected override IEnumerable<MonoBehaviour> GetInjectableMonoBehaviours()
        {
            // We inject on all components on the root except ourself
            foreach (var monoBehaviour in GetComponents<MonoBehaviour>())
            {
                if (monoBehaviour == null)
                {
                    // Missing script
                    continue;
                }

                if (monoBehaviour.GetType().DerivesFrom<MonoInstaller>())
                {
                    // Do not inject on installers since these are always injected before they are installed
                    continue;
                }

                if (monoBehaviour == this)
                {
                    continue;
                }

                yield return monoBehaviour;
            }

            foreach (var monoBehaviour in UnityUtil.GetDirectChildren(this.gameObject)
                .SelectMany<GameObject, MonoBehaviour>(ZenUtilInternal.GetInjectableMonoBehaviours))
            {
                yield return monoBehaviour;
            }
        }

        void InstallBindings()
        {
            _container.DefaultParent = this.transform;

            _container.Bind<Context>().FromInstance(this);
            _container.Bind<GameObjectContext>().FromInstance(this);

            if (_kernel == null)
            {
                _container.Bind<MonoKernel>()
                    .To<DefaultGameObjectKernel>().FromNewComponentOn(this.gameObject).AsSingle().NonLazy();
            }
            else
            {
                _container.Bind<MonoKernel>().FromInstance(_kernel).AsSingle().NonLazy();
            }

            InstallSceneBindings();
            InstallInstallers();
        }
    }
}

#endif
