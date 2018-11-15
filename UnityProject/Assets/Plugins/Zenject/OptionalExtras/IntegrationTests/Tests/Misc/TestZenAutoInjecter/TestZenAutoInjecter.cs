using System.Collections;
using System.Linq;
using ModestTree;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.TestTools;
#endif

namespace Zenject.Tests.AutoInjecter
{
#if UNITY_EDITOR
    public class TestZenAutoInjecter : ZenjectIntegrationTestFixture
    {
        GameObject GetPrefab(string name)
        {
            return FixtureUtil.GetPrefab("TestZenAutoInjecter/{0}".Fmt(name));
        }

        [UnityTest]
        public IEnumerator TestAddComponent()
        {
            PreInstall();

            Container.Bind<Foo>().AsSingle();

            PostInstall();

            var bar = new GameObject("bar").AddComponent<Bar>();

            Assert.That(!bar.ConstructCalled);
            Assert.IsNull(bar.Foo);

            bar.gameObject.AddComponent<ZenAutoInjecter>();

            Assert.IsEqual(bar.Foo, Container.Resolve<Foo>());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefab()
        {
            PreInstall();

            Container.Bind<Foo>().AsSingle();

            PostInstall();
            yield return null;

            var barGameObject = GameObject.Instantiate(GetPrefab("Bar"));
            var bar = barGameObject.GetComponentInChildren<Bar>();

            Assert.IsEqual(bar.Foo, Container.Resolve<Foo>());
            Assert.That(bar.ConstructCalled);
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabWithSearchContainerSourceInGameObjectContext()
        {
            PreInstall();
            Container.Bind<Gorp>().FromSubContainerResolve().ByNewContextPrefab(GetPrefab("GorpContext")).AsSingle();
            PostInstall();
            yield return null;

            var gorp = Container.Resolve<Gorp>();

            var qux = GameObject.Instantiate(
                GetPrefab("QuxSearch"), Vector3.zero, Quaternion.identity, gorp.transform)
                .GetComponentInChildren<Qux>();

            Assert.IsEqual(qux.Container, gorp.Container);
            Assert.IsEqual(qux.Container.ParentContainers.Single(), Container);
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabWithSearchContainerSourceInScene()
        {
            SkipInstall();
            yield return null;

            var qux = GameObject.Instantiate(GetPrefab("QuxSearch")).GetComponentInChildren<Qux>();

            Assert.IsEqual(qux.Container, Container);
            Assert.IsEqual(qux.Container, Container.Resolve<SceneContext>().Container);
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabWithSearchContainerSourceInDontDestroyOnLoad()
        {
            SkipInstall();
            yield return null;

            var qux = GameObject.Instantiate(
                GetPrefab("QuxSearch"), Vector3.zero, Quaternion.identity, ProjectContext.Instance.transform)
                .GetComponentInChildren<Qux>();

            Assert.IsEqual(qux.Container, ProjectContext.Instance.Container);
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabWithProjectContainerSourceInScene()
        {
            SkipInstall();
            yield return null;

            var qux = GameObject.Instantiate(GetPrefab("QuxProject")).GetComponentInChildren<Qux>();

            Assert.IsEqual(qux.Container, ProjectContext.Instance.Container);
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabWithSceneContainerSourceInScene()
        {
            SkipInstall();
            yield return null;

            var qux = GameObject.Instantiate(GetPrefab("QuxScene")).GetComponentInChildren<Qux>();

            Assert.IsEqual(qux.Container, Container);
            Assert.IsEqual(qux.Container, Container.Resolve<SceneContext>().Container);
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabWithSceneContainerSourceInProject()
        {
            SkipInstall();
            yield return null;

            var qux = GameObject.Instantiate(
                GetPrefab("QuxScene"), Vector3.zero, Quaternion.identity,
                ProjectContext.Instance.transform).GetComponentInChildren<Qux>();

            Assert.IsEqual(qux.Container, ProjectContext.Instance.Container);
        }
    }
#endif
}

