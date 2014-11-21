// Ignore the fact that we don't use _dependencyRoot
#pragma warning disable 414

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Define this class as a component of a top-level game object of your scene heirarchy
    // Then any children will get injected during resolve stage
    public sealed class CompositionRoot : MonoBehaviour
    {
        public static MonoInstaller[] CodeInstallers;
        public static Action<DiContainer> ExtraBindingsLookup;

        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        [SerializeField]
        public MonoInstaller[] Installers;

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public void Awake()
        {
            Log.Debug("Zenject Started");

            _container = CreateContainer(false, GlobalCompositionRoot.Instance.Container);

            InjectionHelper.InjectChildGameObjects(_container, gameObject);
            _dependencyRoot = _container.Resolve<IDependencyRoot>();
        }

        public DiContainer CreateContainer(bool allowNullBindings, DiContainer parentContainer)
        {
            var container = new DiContainer();
            container.AllowNullBindings = allowNullBindings;
            container.FallbackProvider = new DiContainerProvider(parentContainer);

            // Install the extra bindings immediately in case they configure the
            // installers used in this scene
            if (ExtraBindingsLookup != null)
            {
                ExtraBindingsLookup(container);

                // Reset extra bindings for next time we change scenes
                ExtraBindingsLookup = null;
            }

            container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();
            container.Bind<GameObject>().To(this.gameObject).WhenInjectedInto<StandardUnityInstaller>();
            container.InstallInstallers();
            Assert.That(!container.HasBinding<IInstaller>());

            InstallSceneInstallers(container);

            return container;
        }

        void InstallSceneInstallers(DiContainer container)
        {
            MonoInstaller[] sceneInstallers=null;
            if (CodeInstallers!=null)
            {
                if (Installers != null)
                    sceneInstallers = CodeInstallers.Concat(CodeInstallers).ToArray();
                else
                    sceneInstallers = CodeInstallers;
            }

            if (sceneInstallers.Where(x => x != null).IsEmpty())
            {
                Log.Warn("No installers found while initializing CompositionRoot");
                return;
            }

            foreach (var installer in sceneInstallers)
            {
                if (installer == null)
                {
                    Log.Warn("Found null installer hooked up to CompositionRoot");
                    continue;
                }

                if (installer.enabled)
                {
                    // The installers that are part of the scene are monobehaviours
                    // and therefore were not created via Zenject and therefore do
                    // not have their members injected
                    // At the very least they will need the container injected but
                    // they might also have some configuration passed from another
                    // scene as well
                    FieldsInjecter.Inject(container, installer);
                    container.Bind<IInstaller>().To(installer);

                    // Install this installer and also any other installers that it installs
                    container.InstallInstallers();

                    Assert.That(!container.HasBinding<IInstaller>());
                }
            }
        }
    }
}
