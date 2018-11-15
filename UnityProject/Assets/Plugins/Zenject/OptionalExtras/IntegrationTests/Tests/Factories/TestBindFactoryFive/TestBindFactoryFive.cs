#if UNITY_EDITOR

using System.Collections;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject.Tests.Factories.BindFactoryFive;

namespace Zenject.Tests.Factories
{
    public class TestBindFactoryFive : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get
            {
                return FixtureUtil.GetPrefab("TestBindFactoryFive/Foo");
            }
        }

        GameObject FooSubContainerPrefab
        {
            get
            {
                return FixtureUtil.GetPrefab("TestBindFactoryFive/FooSubContainer");
            }
        }

        [UnityTest]
        public IEnumerator TestToGameObjectSelf()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, Foo, Foo.Factory>().FromNewComponentOnNewGameObject();

            AddFactoryUser<Foo, Foo.Factory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToGameObjectConcrete()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, IFoo, IFooFactory>().To<Foo>().FromNewComponentOnNewGameObject();

            AddFactoryUser<IFoo, IFooFactory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToMonoBehaviourSelf()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("foo");

            Container.BindFactory<double, int, float, string, char, Foo, Foo.Factory>().FromNewComponentOn(gameObject);

            AddFactoryUser<Foo, Foo.Factory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToMonoBehaviourConcrete()
        {
            PreInstall();
            var gameObject = Container.CreateEmptyGameObject("foo");

            Container.BindFactory<double, int, float, string, char, IFoo, IFooFactory>().To<Foo>().FromNewComponentOn(gameObject);

            AddFactoryUser<IFoo, IFooFactory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToPrefabSelf()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, Foo, Foo.Factory>().FromComponentInNewPrefab(FooPrefab).WithGameObjectName("asdf");

            AddFactoryUser<Foo, Foo.Factory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertNumGameObjectsWithName("asdf", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToPrefabConcrete()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, IFoo, IFooFactory>().To<Foo>().FromComponentInNewPrefab(FooPrefab).WithGameObjectName("asdf");

            AddFactoryUser<IFoo, IFooFactory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertNumGameObjectsWithName("asdf", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToPrefabResourceSelf()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, Foo, Foo.Factory>().FromComponentInNewPrefabResource("TestBindFactoryFive/Foo").WithGameObjectName("asdf");

            AddFactoryUser<Foo, Foo.Factory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertNumGameObjectsWithName("asdf", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToPrefabResourceConcrete()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, IFoo, IFooFactory>()
                .To<Foo>().FromComponentInNewPrefabResource("TestBindFactoryFive/Foo").WithGameObjectName("asdf");

            AddFactoryUser<IFoo, IFooFactory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertNumGameObjectsWithName("asdf", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToSubContainerPrefabSelf()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, Foo, Foo.Factory>()
                .FromSubContainerResolve().ByNewContextPrefab<FooInstaller>(FooSubContainerPrefab);

            AddFactoryUser<Foo, Foo.Factory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToSubContainerPrefabConcrete()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, IFoo, IFooFactory>()
                .To<Foo>().FromSubContainerResolve().ByNewContextPrefab<FooInstaller>(FooSubContainerPrefab);

            AddFactoryUser<IFoo, IFooFactory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToSubContainerPrefabResourceSelf()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, Foo, Foo.Factory>().FromSubContainerResolve().ByNewContextPrefabResource<FooInstaller>("TestBindFactoryFive/FooSubContainer");

            AddFactoryUser<Foo, Foo.Factory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestToSubContainerPrefabResourceConcrete()
        {
            PreInstall();
            Container.BindFactory<double, int, float, string, char, IFoo, IFooFactory>()
                .To<Foo>().FromSubContainerResolve().ByNewContextPrefabResource<FooInstaller>("TestBindFactoryFive/FooSubContainer");

            AddFactoryUser<IFoo, IFooFactory>();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        void AddFactoryUser<TValue, TFactory>()
            where TValue : IFoo
            where TFactory : PlaceholderFactory<double, int, float, string, char, TValue>
        {
            Container.Bind<IInitializable>()
                .To<FooFactoryTester<TValue, TFactory>>().AsSingle();

            Container.BindExecutionOrder<FooFactoryTester<TValue, TFactory>>(-100);
        }

        public class FooFactoryTester<TValue, TFactory> : IInitializable
            where TFactory : PlaceholderFactory<double, int, float, string, char, TValue>
            where TValue : IFoo
        {
            readonly TFactory _factory;

            public FooFactoryTester(TFactory factory)
            {
                _factory = factory;
            }

            public void Initialize()
            {
                Assert.IsEqual(_factory.Create(0.15, 0, 2.4f, "zxcv", 'z').Value, "zxcv");

                Log.Info("Factory created foo successfully");
            }
        }
    }
}

#endif
