#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject.Tests.Bindings.DiContainerMethods;

namespace Zenject.Tests.Bindings
{
    public class TestDiContainerMethods : ZenjectIntegrationTestFixture
    {
        const string ResourcePrefix = "TestDiContainerMethods/";

        GameObject FooPrefab
        {
            get { return GetPrefab("Foo"); }
        }

        GameObject GorpPrefab
        {
            get { return GetPrefab("Gorp"); }
        }

        GameObject CameraPrefab
        {
            get { return GetPrefab("Camera"); }
        }

        [UnityTest]
        public IEnumerator TestInstantiateComponent()
        {
            SkipInstall();

            var gameObject = new GameObject();

            var foo = Container.InstantiateComponent<Foo>(gameObject);

            Assert.That(foo.WasInjected);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiateComponentArgs()
        {
            SkipInstall();

            var gameObject = new GameObject();

            Assert.Throws(() => Container.InstantiateComponent<Gorp>(gameObject));

            var gorp = Container.InstantiateComponent<Gorp>(gameObject, new object[] { "zxcv" });

            Assert.IsEqual(gorp.Arg, "zxcv");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiateComponentOnNewGameObject()
        {
            SkipInstall();

            var foo = Container.InstantiateComponentOnNewGameObject<Foo>();

            Assert.That(foo.WasInjected);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiateComponentOnNewGameObjectArgs()
        {
            SkipInstall();

            Assert.Throws(() => Container.InstantiateComponentOnNewGameObject<Gorp>());

            var gorp = Container.InstantiateComponentOnNewGameObject<Gorp>("sdf", new object[] { "zxcv" });

            Assert.IsEqual(gorp.Arg, "zxcv");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefab()
        {
            SkipInstall();

            var go = Container.InstantiatePrefab(FooPrefab);

            var foo = go.GetComponentInChildren<Foo>();

            Assert.That(foo.WasInjected);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabForMonoBehaviour()
        {
            SkipInstall();

            Assert.Throws(() => Container.InstantiatePrefab(GorpPrefab));

            var gorp = Container.InstantiatePrefabForComponent<Gorp>(GorpPrefab, new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabResource()
        {
            SkipInstall();

            Assert.Throws(() => Container.InstantiatePrefabResource(ResourcePrefix + "Gorp"));

            var gorp = Container.InstantiatePrefabResourceForComponent<Gorp>(ResourcePrefix + "Gorp", new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabForComponent()
        {
            SkipInstall();

            var camera = Container.InstantiatePrefabForComponent<Camera>(CameraPrefab, new object[0]);
            Assert.IsNotNull(camera);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabForComponentMistake()
        {
            SkipInstall();

            Assert.Throws(() => Container.InstantiatePrefabForComponent<Camera>(CameraPrefab, new object[] { "sdf" }));
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiateScriptableObjectResource()
        {
            SkipInstall();

            var foo = Container.InstantiateScriptableObjectResource<Foo2>(ResourcePrefix + "Foo2");
            Assert.That(foo.WasInjected);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiateScriptableObjectResourceArgs()
        {
            SkipInstall();

            Assert.Throws(() => Container.InstantiateScriptableObjectResource<Gorp2>(ResourcePrefix + "Gorp2"));

            var gorp = Container.InstantiateScriptableObjectResource<Gorp2>(ResourcePrefix + "Gorp2", new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInjectGameObject()
        {
            SkipInstall();

            var go = GameObject.Instantiate(FooPrefab);

            var foo = go.GetComponentInChildren<Foo>();

            Assert.That(!foo.WasInjected);
            Container.InjectGameObject(go);
            Assert.That(foo.WasInjected);

            yield break;
        }

        [UnityTest]
        public IEnumerator TestInjectGameObjectForMonoBehaviour()
        {
            SkipInstall();

            var go = GameObject.Instantiate(GorpPrefab);

            Assert.Throws(() => Container.InjectGameObject(go));

            var gorp = Container.InjectGameObjectForComponent<Gorp>(go, new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInjectGameObjectForComponent()
        {
            SkipInstall();

            var go = GameObject.Instantiate(CameraPrefab);

            Container.InjectGameObjectForComponent<Camera>(go, new object[0]);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInjectGameObjectForComponentMistake()
        {
            SkipInstall();

            var go = GameObject.Instantiate(CameraPrefab);

            Assert.Throws(() => Container.InjectGameObjectForComponent<Camera>(go, new object[] { "sdf" }));
            yield break;
        }

        [UnityTest]
        public IEnumerator TestLazyInstanceInjectorFail()
        {
            PreInstall();
            Qux.WasInjected = false;

            var qux = new Qux();
            Container.BindInstance(qux);

            Assert.That(!Qux.WasInjected);
            PostInstall();
            Assert.That(!Qux.WasInjected);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestLazyInstanceInjectorSuccess()
        {
            PreInstall();
            Qux.WasInjected = false;

            var qux = new Qux();
            Container.BindInstance(qux);
            Container.QueueForInject(qux);

            Assert.That(!Qux.WasInjected);
            PostInstall();
            Assert.That(Qux.WasInjected);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabForComponentExplicit()
        {
            SkipInstall();

            var parentGameObject = new GameObject();
            parentGameObject.transform.position = new Vector3(100, 100, 100);
            var parentTransform = parentGameObject.transform;

            var go = (Foo)Container.InstantiatePrefabForComponentExplicit(typeof(Foo), FooPrefab, new List<TypeValuePair>(), new GameObjectCreationParameters { ParentTransform = parentTransform });

            var foo = go.GetComponentInChildren<Foo>();

            Assert.IsEqual(foo.transform.position, new Vector3(100, 100, 100));
            yield break;
        }

        [UnityTest]
        public IEnumerator TestInstantiatePrefabForComponentWithPositionExplicit()
        {
            SkipInstall();

            var parentGameObject = new GameObject();
            parentGameObject.transform.position = new Vector3(100, 100, 100);
            parentGameObject.transform.rotation = Quaternion.Euler(10, 10, 10);
            var parentTransform = parentGameObject.transform;

            var go = (Foo)Container.InstantiatePrefabForComponentExplicit(typeof(Foo), FooPrefab, new List<TypeValuePair>(), new GameObjectCreationParameters
            {
                ParentTransform = parentTransform,
                Position = new Vector3(50, 50, 50),
                Rotation = Quaternion.Euler(20, 20, 20)
            });

            var foo = go.GetComponentInChildren<Foo>();

            Assert.That(Approximately(foo.transform.position, new Vector3(50, 50, 50)));
            Assert.That(Approximately(foo.transform.rotation.eulerAngles, new Vector3(20, 20, 20)));
            yield break;
        }

        static bool Approximately(Vector3 left, Vector3 right)
        {
            return Mathf.Approximately(left.x, right.x)
                && Mathf.Approximately(left.y, right.y)
                && Mathf.Approximately(left.z, right.z);
        }

        public class Qux
        {
            public static bool WasInjected
            {
                get;
                set;
            }

            [Inject]
            public void Construct()
            {
                WasInjected = true;
            }
        }

        GameObject GetPrefab(string name)
        {
            return FixtureUtil.GetPrefab(ResourcePrefix + name);
        }
    }
}

#endif
