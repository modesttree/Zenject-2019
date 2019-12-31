using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Zenject.Internal;
using Assert = ModestTree.Assert;

namespace Zenject {
    public abstract class EditorSceneTestFixture {
        private readonly List<DiContainer> _sceneContainers = new List<DiContainer>();

        private bool _hasLoadedScene;

        protected DiContainer SceneContainer { get; private set; }
        protected IEnumerable<DiContainer> SceneContainers => _sceneContainers;

        public IEnumerator LoadScene(string scenePath) {
            return LoadScenes(scenePath);
        }

        public IEnumerator LoadScenes(params string[] scenePaths) {
            Assert.That(!_hasLoadedScene, "Attempted to load scene twice!");
            _hasLoadedScene = true;

            // Clean up any leftovers from previous test
            ZenjectTestUtil.DestroyEverythingExceptTestRunner(false);

            Assert.That(SceneContainers.IsEmpty());

            for (var i = 0; i < scenePaths.Length; i++) {
                var scenePath = scenePaths[i];

                Log.Info("Loading scene '{0}' for testing", scenePath);

                var loader =
                    EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(
                        i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive
                    ));

                while (!loader.isDone) {
                    yield return null;
                }

                SceneContext sceneContext = null;

                // ProjectContext might be null if scene does not have a scene context
                if (ProjectContext.HasInstance) {
                    var scene = SceneManager.GetSceneByPath(scenePath);

                    sceneContext = ProjectContext.Instance.Container.Resolve<SceneContextRegistry>()
                        .TryGetSceneContextForScene(scene);
                }

                _sceneContainers.Add(sceneContext == null ? null : sceneContext.Container);
            }

            SceneContainer = _sceneContainers.LastOrDefault(x => x != null);

            SceneContainer?.Inject(this);
        }

        [SetUp]
        public virtual void SetUp() {
            StaticContext.Clear();
            SetMemberDefaults();
        }

        private void SetMemberDefaults() {
            _hasLoadedScene = false;
            SceneContainer = null;
            _sceneContainers.Clear();
        }

        [TearDown]
        public virtual void Teardown() {
            ZenjectTestUtil.DestroyEverythingExceptTestRunner(true);
            StaticContext.Clear();
            SetMemberDefaults();
        }
    }
}