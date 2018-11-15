#if UNITY_EDITOR

using System.Collections;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject.Tests.Bindings.FromSubContainerPrefab;

#pragma warning disable 649

namespace Zenject.Tests.Bindings
{
    public class TestFromSubContainerPrefab : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get { return FixtureUtil.GetPrefab("TestFromSubContainerPrefab/Foo"); }
        }

        GameObject FooPrefab2
        {
            get { return FixtureUtil.GetPrefab("TestFromSubContainerPrefab/Foo2"); }
        }

        void CommonInstall()
        {
            Container.Settings = new ZenjectSettings(ValidationErrorResponses.Throw);
        }

        [UnityTest]
        public IEnumerator TestSelfSingle()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Foo>().FromSubContainerResolve()
                .ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        [ValidateOnly]
        public IEnumerator TestSelfSingleValidate()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Foo>().FromSubContainerResolve()
                .ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();
            yield break;
        }

        [UnityTest]
        [ValidateOnly]
        public IEnumerator TestSelfSingleValidateFails()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Foo>().FromSubContainerResolve()
                .ByNewContextPrefab(FooPrefab2).AsSingle().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfTransient()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Foo>().FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfCached()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Foo>().FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfSingleMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfCachedMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfTransientMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(2);
            FixtureUtil.AssertComponentCount<Foo>(2);
            FixtureUtil.AssertComponentCount<Bar>(2);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteSingle()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteTransient()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve()
                .ByNewContextPrefab(FooPrefab).AsTransient().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteCached()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<IFoo>().To<Foo>().FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteSingleMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(Bar), typeof(IFoo)).To(typeof(Foo), typeof(Bar))
                .FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            FixtureUtil.AssertComponentCount<Bar>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestConcreteCachedMultipleContracts()
        {
            PreInstall();
            CommonInstall();

            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            FixtureUtil.AssertComponentCount<Foo>(1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfIdentifiersFails()
        {
            PreInstall();
            CommonInstall();

            Container.Bind<Gorp>().FromSubContainerResolve().ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSelfIdentifiers()
        {
            PreInstall();
            CommonInstall();

            Container.Settings = new ZenjectSettings(ValidationErrorResponses.Throw);
            Container.Bind<Gorp>().FromSubContainerResolve("gorp").ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            FixtureUtil.AssertNumGameObjects(1);
            yield break;
        }
    }
}

#endif
