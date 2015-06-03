#if !ZEN_NOT_UNITY3D

#pragma warning disable 414
using ModestTree;

using System;
using System.Collections.Generic;
using ModestTree.Util.Debugging;
using System.Linq;
using UnityEngine;

namespace Zenject
{
    // Define this class as a component of a top-level game object of your scene heirarchy
    // Then any children will get injected during resolve stage
    public sealed class CompositionRoot : MonoBehaviour
    {
        public static Action<DiContainer> BeforeInstallHooks;
        public static Action<DiContainer> AfterInstallHooks;

        public bool OnlyInjectWhenActive = true;
        public bool InjectFullScene = false;

        [SerializeField]
        public MonoInstaller[] Installers = new MonoInstaller[0];

        DiContainer _container;
        IDependencyRoot _dependencyRoot = null;

        static List<IInstaller> _staticInstallers = new List<IInstaller>();

        public DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        // This method is used for cases where you need to create the CompositionRoot entirely in code
        // Necessary because the Awake() method is called immediately after AddComponent<CompositionRoot>
        // so there's no other way to add installers to it
        public static CompositionRoot AddComponent(
            GameObject gameObject, IInstaller rootInstaller)
        {
            return AddComponent(gameObject, new List<IInstaller>() { rootInstaller });
        }

        public static CompositionRoot AddComponent(
            GameObject gameObject, List<IInstaller> installers)
        {
            Assert.That(_staticInstallers.IsEmpty());
            _staticInstallers.AddRange(installers);
            return gameObject.AddComponent<CompositionRoot>();
        }

        public void Awake()
        {
            if (Container != null)
                return;

            _container = CreateContainer(
                false, GlobalCompositionRoot.Instance.Container, _staticInstallers);
            _staticInstallers.Clear();

            if (InjectFullScene)
            {
                var rootGameObjects = GameObject.FindObjectsOfType<Transform>()
                    .Where(x => x.parent == null && x.GetComponent<GlobalCompositionRoot>() == null && (x.GetComponent<CompositionRoot>() == null || x == this.transform))
                    .Select(x => x.gameObject).ToList();

                foreach (var rootObj in rootGameObjects)
                {
                    _container.InjectGameObject(rootObj, true, !OnlyInjectWhenActive);
                }
            }
            else
            {
                _container.InjectGameObject(gameObject, true, !OnlyInjectWhenActive);
            }

            _dependencyRoot = _container.Resolve<IDependencyRoot>();
        }

        public DiContainer CreateContainer(
            bool allowNullBindings, DiContainer parentContainer, List<IInstaller> extraInstallers)
        {
            var container = new DiContainer();
            container.AllowNullBindings = allowNullBindings;
            container.FallbackProvider = new DiContainerProvider(parentContainer);
            container.Bind<CompositionRoot>().ToInstance(this);

            if (BeforeInstallHooks != null)
            {
                BeforeInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                BeforeInstallHooks = null;
            }

            CompositionRootHelper.InstallStandardInstaller(container, this.gameObject);

            var allInstallers = extraInstallers.Concat(Installers).ToList();

            if (allInstallers.Where(x => x != null).IsEmpty())
            {
                Log.Warn("No installers found while initializing CompositionRoot");
            }
            else
            {
                CompositionRootHelper.InstallSceneInstallers(container, allInstallers);
            }

            if (AfterInstallHooks != null)
            {
                AfterInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                AfterInstallHooks = null;
            }

            return container;
        }
    }
}

#endif
