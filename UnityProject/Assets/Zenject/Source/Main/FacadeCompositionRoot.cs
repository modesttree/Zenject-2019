#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;
using Zenject.Internal;

namespace Zenject
{
    public class FacadeCompositionRoot : CompositionRoot
    {
        [SerializeField]
        MonoFacade _facade = null;

        DiContainer _container;

        public override IDependencyRoot DependencyRoot
        {
            get
            {
                return _facade;
            }
        }

        public MonoFacade Facade
        {
            get
            {
                return _facade;
            }
        }

        [PostInject]
        public void Construct(
            DiContainer parentContainer,
            [InjectOptional]
            InstallerExtraArgs installerExtraArgs)
        {
            Assert.IsNull(_container);

            if (_facade == null)
            {
                throw new ZenjectBindException(
                    "Facade property is not set in FacadeCompositionRoot '{0}'".Fmt(this.gameObject.name));
            }

            if (!UnityUtil.GetParentsAndSelf(_facade.transform).Contains(this.transform))
            {
                throw new ZenjectBindException(
                    "The given Facade must exist on the same game object as the FacadeCompositionRoot '{0}' or a descendant!".Fmt(this.name));
            }

            Log.Debug("FacadeCompositionRoot: Running installers...");

            _container = parentContainer.CreateSubContainer();

            _container.IsInstalling = true;

            try
            {
                InstallBindings(_container, installerExtraArgs);
            }
            finally
            {
                _container.IsInstalling = false;
            }

            Log.Debug("FacadeCompositionRoot: Injecting into child components...");

            InjectComponents(_container);

            Log.Debug("FacadeCompositionRoot: Initialized successfully");
        }

        void InjectComponents(DiContainer container)
        {
            // Use ToList in case they do something weird in post inject
            foreach (var component in GetInjectableComponents().ToList())
            {
                Assert.That(!component.GetType().DerivesFrom<MonoInstaller>());

                container.Inject(component);
            }
        }

        public override IEnumerable<Component> GetInjectableComponents()
        {
            // We inject on all components on the root except ourself
            foreach (var component in GetComponents<Component>())
            {
                if (component == null)
                {
                    Log.Warn("Zenject: Found null component on game object '{0}'.  Possible missing script.", gameObject.name);
                    continue;
                }

                if (component.GetType().DerivesFrom<MonoInstaller>())
                {
                    // Do not inject on installers since these are always injected before they are installed
                    continue;
                }

                if (component == this)
                {
                    continue;
                }

                yield return component;
            }

            foreach (var gameObject in UnityUtil.GetDirectChildren(this.gameObject))
            {
                foreach (var component in GetInjectableComponents(gameObject, OnlyInjectWhenActive))
                {
                    yield return component;
                }
            }
        }

        public void InstallBindings(
            DiContainer container, InstallerExtraArgs installerExtraArgs)
        {
            container.Bind<CompositionRoot>().ToInstance(this);
            container.Bind<Transform>(DiContainer.DefaultParentId)
                .ToInstance<Transform>(this.transform);

            container.Bind<IDependencyRoot>().ToInstance(_facade);

            InstallSceneBindings(container);

            var extraArgsMap = new Dictionary<Type, List<TypeValuePair>>();

            if (installerExtraArgs != null)
            {
                extraArgsMap.Add(
                    installerExtraArgs.InstallerType, installerExtraArgs.ExtraArgs);
            }

            InstallInstallers(container, extraArgsMap);
        }

        public class InstallerExtraArgs
        {
            public Type InstallerType;
            public List<TypeValuePair> ExtraArgs;
        }
    }
}

#endif
