using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using ModestTree;
using Assert=ModestTree.Assert;
using Zenject.Tests.Bindings.DiContainerMethods;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
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

        [Test]
        public void TestInstantiateComponent()
        {
            Initialize();

            var gameObject = new GameObject();

            var foo = Container.InstantiateComponent<Foo>(gameObject);

            Assert.That(foo.WasInjected);
        }

        [Test]
        public void TestInstantiateComponentArgs()
        {
            Initialize();

            var gameObject = new GameObject();

            Assert.Throws(() => Container.InstantiateComponent<Gorp>(gameObject));

            var gorp = Container.InstantiateComponent<Gorp>(gameObject, new object[] { "zxcv" });

            Assert.IsEqual(gorp.Arg, "zxcv");
        }

        [Test]
        public void TestInstantiateComponentOnNewGameObject()
        {
            Initialize();

            var foo = Container.InstantiateComponentOnNewGameObject<Foo>();

            Assert.That(foo.WasInjected);
        }

        [Test]
        public void TestInstantiateComponentOnNewGameObjectArgs()
        {
            Initialize();

            Assert.Throws(() => Container.InstantiateComponentOnNewGameObject<Gorp>());

            var gorp = Container.InstantiateComponentOnNewGameObject<Gorp>("sdf", new object[] { "zxcv" });

            Assert.IsEqual(gorp.Arg, "zxcv");
        }

        [Test]
        public void TestInstantiatePrefab()
        {
            Initialize();

            var go = Container.InstantiatePrefab(FooPrefab);

            var foo = go.GetComponentInChildren<Foo>();

            Assert.That(foo.WasInjected);
        }

        [Test]
        public void TestInstantiatePrefabForMonoBehaviour()
        {
            Initialize();

            Assert.Throws(() => Container.InstantiatePrefab(GorpPrefab));

            var gorp = Container.InstantiatePrefabForComponent<Gorp>(GorpPrefab, new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
        }

        [Test]
        public void TestInstantiatePrefabResource()
        {
            Initialize();

            Assert.Throws(() => Container.InstantiatePrefabResource(ResourcePrefix + "Gorp"));

            var gorp = Container.InstantiatePrefabResourceForComponent<Gorp>(ResourcePrefix + "Gorp", new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
        }

        [Test]
        public void TestInstantiatePrefabForComponent()
        {
            Initialize();

            var camera = Container.InstantiatePrefabForComponent<Camera>(CameraPrefab, new object[0]);
            Assert.IsNotNull(camera);
        }

        [Test]
        public void TestInstantiatePrefabForComponentMistake()
        {
            Initialize();

            Assert.Throws(() => Container.InstantiatePrefabForComponent<Camera>(CameraPrefab, new object[] { "sdf" }));
        }

        [Test]
        public void TestInstantiateScriptableObjectResource()
        {
            Initialize();

            var foo = Container.InstantiateScriptableObjectResource<Foo2>(ResourcePrefix + "Foo2");
            Assert.That(foo.WasInjected);
        }

        [Test]
        public void TestInstantiateScriptableObjectResourceArgs()
        {
            Initialize();

            Assert.Throws(() => Container.InstantiateScriptableObjectResource<Gorp2>(ResourcePrefix + "Gorp2"));

            var gorp = Container.InstantiateScriptableObjectResource<Gorp2>(ResourcePrefix + "Gorp2", new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
        }

        [Test]
        public void TestInjectGameObject()
        {
            Initialize();

            var go = GameObject.Instantiate(FooPrefab);

            var foo = go.GetComponentInChildren<Foo>();

            Assert.That(!foo.WasInjected);
            Container.InjectGameObject(go);
            Assert.That(foo.WasInjected);
        }

        [Test]
        public void TestInjectGameObjectForMonoBehaviour()
        {
            Initialize();

            var go = GameObject.Instantiate(GorpPrefab);

            Assert.Throws(() => Container.InjectGameObject(go));

            var gorp = Container.InjectGameObjectForComponent<Gorp>(go, new object[] { "asdf" });

            Assert.IsEqual(gorp.Arg, "asdf");
        }

        [Test]
        public void TestInjectGameObjectForComponent()
        {
            Initialize();

            var go = GameObject.Instantiate(CameraPrefab);

            Container.InjectGameObjectForComponent<Camera>(go, new object[0]);
        }

        [Test]
        public void TestInjectGameObjectForComponentMistake()
        {
            Initialize();

            var go = GameObject.Instantiate(CameraPrefab);

            Assert.Throws(() => Container.InjectGameObjectForComponent<Camera>(go, new object[] { "sdf" }));
        }

        [Test]
        public void TestLazyInstanceInjectorFail()
        {
            Qux.WasInjected = false;

            var qux = new Qux();
            Container.BindInstance(qux);

            Assert.That(!Qux.WasInjected);
            Initialize();
            Assert.That(!Qux.WasInjected);
        }

        [Test]
        public void TestLazyInstanceInjectorSuccess()
        {
            Qux.WasInjected = false;

            var qux = new Qux();
            Container.BindInstance(qux);
            Container.QueueForInject(qux);

            Assert.That(!Qux.WasInjected);
            Initialize();
            Assert.That(Qux.WasInjected);
        }

        [Test]
        public void TestInstantiatePrefabForComponentExplicit()
        {
            Initialize();

            var parentGameObject = new GameObject();
            parentGameObject.transform.position = new Vector3(100, 100, 100);
            var parentTransform = parentGameObject.transform;

            var go = (Foo)Container.InstantiatePrefabForComponentExplicit(typeof(Foo), FooPrefab, new List<TypeValuePair>(), new GameObjectCreationParameters() { ParentTransform = parentTransform });

            var foo = go.GetComponentInChildren<Foo>();

            Assert.IsEqual(foo.transform.position, new Vector3(100, 100, 100));
        }

        [Test]
        public void TestInstantiatePrefabForComponentWithPositionExplicit()
        {
            Initialize();

            var parentGameObject = new GameObject();
            parentGameObject.transform.position = new Vector3(100, 100, 100);
            var parentTransform = parentGameObject.transform;

            var go = (Foo)Container.InstantiatePrefabForComponentExplicit(typeof(Foo), FooPrefab, new List<TypeValuePair>(), new GameObjectCreationParameters()
            {
                ParentTransform = parentTransform,
                Position = new Vector3(50, 50, 50)
            });

            var foo = go.GetComponentInChildren<Foo>();

            Assert.IsEqual(foo.transform.position, new Vector3(50, 50, 50));
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

