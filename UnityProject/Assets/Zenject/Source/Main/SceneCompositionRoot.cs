#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject.Internal;

#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif

namespace Zenject
{
    public class SceneCompositionRoot : CompositionRoot
    {
        public static readonly List<Scene> DecoratedScenes = new List<Scene>();

        public static Action<DiContainer> BeforeInstallHooks;
        public static Action<DiContainer> AfterInstallHooks;

        [FormerlySerializedAs("ParentNewObjectsUnderRoot")]
        [Tooltip("When true, objects that are created at runtime will be parented to the SceneCompositionRoot")]
        [SerializeField]
        bool _parentNewObjectsUnderRoot = false;

        DiContainer _container;
        IDependencyRoot _dependencyRoot;

        static StaticSettings _staticSettings;

        public override IDependencyRoot DependencyRoot
        {
            get
            {
                return _dependencyRoot;
            }
        }

        public void Awake()
        {
            if (_staticSettings != null)
            // Static settings are needed if creating a SceneCompositionRoot dynamically
            {
                this.SetInstallers(_staticSettings.Installers);
                OnlyInjectWhenActive = _staticSettings.OnlyInjectWhenActive;
                _parentNewObjectsUnderRoot = _staticSettings.ParentNewObjectsUnderRoot;
                _staticSettings = null;
            }

            // We always want to initialize GlobalCompositionRoot as early as possible
            GlobalCompositionRoot.Instance.EnsureIsInitialized();

            Assert.IsNull(_container);

            _container = new DiContainer(
                GlobalCompositionRoot.Instance.Container, false);

            // This can be valid in cases where you have everything in either facade installers
            // or global installers so just ignore
            //if (Installers.IsEmpty() && InstallerPrefabs.IsEmpty())
            //{
                //Log.Warn("No installers found while initializing CompositionRoot '{0}'", this.name);
            //}

            Log.Debug("SceneCompositionRoot: Running installers...");

            _container.IsInstalling = true;

            try
            {
                InstallBindings(_container);
            }
            finally
            {
                _container.IsInstalling = false;
            }

            Log.Debug("SceneCompositionRoot: Injecting components in the scene...");

            InjectComponents(_container);

            Log.Debug("SceneCompositionRoot: Resolving dependency root...");

            _dependencyRoot = _container.Resolve<IDependencyRoot>();

            DecoratedScenes.Clear();

            Log.Debug("SceneCompositionRoot: Initialized successfully");
        }

        // We pass in the container here instead of using our own for validation to work
        public void InstallBindings(DiContainer container)
        {
            if (_parentNewObjectsUnderRoot)
            {
                container.Bind<Transform>(DiContainer.DefaultParentId)
                    .ToInstance<Transform>(this.transform);
            }

            container.Bind<CompositionRoot>().ToInstance(this);

            InstallSceneBindings(container);

            if (BeforeInstallHooks != null)
            {
                BeforeInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                BeforeInstallHooks = null;
            }

            container.Bind<IDependencyRoot>().ToSingleMonoBehaviour<SceneFacade>(this.gameObject);

            InstallInstallers(container);

            if (AfterInstallHooks != null)
            {
                AfterInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                AfterInstallHooks = null;
            }
        }

        public override IEnumerable<Component> GetInjectableComponents()
        {
            foreach (var gameObject in GetRootGameObjects())
            {
                foreach (var component in GetInjectableComponents(gameObject, OnlyInjectWhenActive))
                {
                    yield return component;
                }
            }
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

        IEnumerable<GameObject> GetRootGameObjects()
        {
            var scene = this.gameObject.scene;
            // Note: We can't use activeScene.GetRootObjects() here because that apparently fails with an exception
            // about the scene not being loaded yet when executed in Awake
            // It's important here that we only inject into root objects that are part of our scene
            // Otherwise, if there is an object that is marked with DontDestroyOnLoad, then it will
            // be injected multiple times when another scene is loaded
            // Also make sure not to inject into the global root objects which are handled in GlobalCompositionRoot
            return GameObject.FindObjectsOfType<Transform>()
                .Where(x => x.parent == null && x.GetComponent<GlobalCompositionRoot>() == null && (x.gameObject.scene == scene || DecoratedScenes.Contains(x.gameObject.scene)))
                .Select(x => x.gameObject);
        }

        // These methods can be used for cases where you need to create the SceneCompositionRoot entirely in code
        // Necessary because the Awake() method is called immediately after InstantiateComponent<SceneCompositionRoot>
        // so there's no other way to add installers to it
        public static SceneCompositionRoot Instantiate(
            GameObject parent, StaticSettings settings)
        {
            var gameObject = new GameObject();

            if (parent != null)
            {
                gameObject.transform.SetParent(parent.transform, false);
            }

            return InstantiateComponent(gameObject, settings);
        }

        public static SceneCompositionRoot InstantiateComponent(
            GameObject gameObject, StaticSettings settings)
        {
            Assert.IsNull(_staticSettings);
            _staticSettings = settings;

            var result = gameObject.AddComponent<SceneCompositionRoot>();
            Assert.IsNull(_staticSettings);
            return result;
        }

        public class StaticSettings
        {
            public MonoInstaller[] Installers;
            public bool ParentNewObjectsUnderRoot;
            public bool OnlyInjectWhenActive;
        }
    }
}

#endif

