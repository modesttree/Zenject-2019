#if UNITY_EDITOR

using System.Collections;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject.Tests.Bindings.FromPrefab;

namespace Zenject.Tests.Bindings
{
    public class TestFromPrefab : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get { return GetPrefab("Foo"); }
        }

        GameObject GorpPrefab
        {
            get { return GetPrefab("Gorp"); }
        }

        GameObject GorpAndQuxPrefab
        {
            get { return GetPrefab("GorpAndQux"); }
        }

        GameObject NorfPrefab
        {
            get { return GetPrefab("Norf"); }
        }

        GameObject JimAndBobPrefab
        {
            get { return GetPrefab("JimAndBob"); }
        }

        [UnityTest]
        public IEnumerator TestTransient()
        {
            PreInstall();
            Container.Bind<Foo>().FromComponentInNewPrefab(FooPrefab).AsTransient().NonLazy();
            Container.Bind<Foo>().FromComponentInNewPrefab(FooPrefab).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertComponentCount<Foo>(2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSingle()
        {
            PreInstall();
            Container.Bind(typeof(IFoo), typeof(Foo)).To<Foo>().FromComponentInNewPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            Assert.IsNotNull(Container.Resolve<IFoo>());
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestCached1()
        {
            PreInstall();
            Container.Bind(typeof(Foo), typeof(Bar)).FromComponentInNewPrefab(FooPrefab)
                .WithGameObjectName("Foo").AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            FixtureUtil.AssertNumGameObjectsWithName("Foo", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithArgumentsFail()
        {
            PreInstall();
            // They have required arguments
            Container.Bind(typeof(Gorp), typeof(Qux)).FromComponentInNewPrefab(GorpAndQuxPrefab).AsSingle().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithArgumentsFail2()
        {
            PreInstall();
            Container.Bind<Gorp>()
                .FromComponentInNewPrefab(GorpAndQuxPrefab).WithGameObjectName("Gorp").AsSingle()
                .WithArguments(5, "test1").NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithArgumentsSuccess()
        {
            PreInstall();
            Container.Bind<Gorp>().FromComponentInNewPrefab(GorpPrefab)
                .WithGameObjectName("Gorp").AsSingle()
                .WithArguments("test1").NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Gorp>(1);
            FixtureUtil.AssertNumGameObjectsWithName("Gorp", 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithAbstractSearchSingleMatch()
        {
            PreInstall();
            // There are three components that implement INorf on this prefab
            Container.Bind<INorf>().FromComponentInNewPrefab(NorfPrefab).AsCached().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<INorf>(3);
            FixtureUtil.AssertResolveCount<INorf>(Container, 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestWithAbstractSearchMultipleMatch()
        {
            PreInstall();
            // There are three components that implement INorf on this prefab
            Container.Bind<INorf>().FromComponentsInNewPrefab(NorfPrefab).AsCached().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<INorf>(3);
            FixtureUtil.AssertResolveCount<INorf>(Container, 3);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestAbstractBindingConcreteSearch()
        {
            PreInstall();
            // Should ignore the Norf2 component on it
            Container.Bind<INorf>().To<Norf>().FromComponentsInNewPrefab(NorfPrefab).AsCached().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertResolveCount<INorf>(Container, 2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestMultipleMatchFailure()
        {
            PreInstall();
            Container.Bind<INorf>().FromComponentsInNewPrefab(FooPrefab).AsSingle().NonLazy();
            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestMultipleMatchTransform()
        {
            PreInstall();
            Container.Bind<Transform>().FromComponentInNewPrefab(FooPrefab).AsCached();
            PostInstall();
            var transform = Container.Resolve<Transform>();
            Assert.IsNotNull(transform);
            Assert.IsNull(transform.parent);
            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestCircularDependencies()
        {
            PreInstall();
            // Jim and Bob both depend on each other
            Container.Bind(typeof(Jim), typeof(Bob)).FromComponentInNewPrefab(JimAndBobPrefab).AsSingle().NonLazy();
            Container.BindInterfacesTo<JimAndBobRunner>().AsSingle().NonLazy();

            PostInstall();
            yield break;
        }

        GameObject GetPrefab(string name)
        {
            return FixtureUtil.GetPrefab("TestFromPrefab/{0}".Fmt(name));
        }

        public class JimAndBobRunner : IInitializable
        {
            readonly Bob _bob;
            readonly Jim _jim;

            public JimAndBobRunner(Jim jim, Bob bob)
            {
                _bob = bob;
                _jim = jim;
            }

            public void Initialize()
            {
                Assert.IsNotNull(_jim.Bob);
                Assert.IsNotNull(_bob.Jim);

                Log.Info("Jim and bob successfully got the other reference");
            }
        }
    }
}

#endif
