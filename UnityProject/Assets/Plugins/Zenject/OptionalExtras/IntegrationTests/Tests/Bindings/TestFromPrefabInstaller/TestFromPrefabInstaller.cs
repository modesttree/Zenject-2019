#if UNITY_EDITOR

using System.Collections;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;

namespace Zenject.Tests.Bindings.FromPrefabInstaller
{
    public class TestFromPrefabInstaller : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get { return FixtureUtil.GetPrefab(FooPrefabResourcePath); }
        }

        string FooPrefabResourcePath
        {
            get { return "TestFromPrefabInstaller/Foo"; }
        }

        [UnityTest]
        public IEnumerator TestInstaller()
        {
            PreInstall();

            Container.Bind<Qux>().FromSubContainerResolve()
                .ByNewPrefabInstaller<FooInstaller>(FooPrefab).AsCached();

            PostInstall();

            Assert.IsEqual(Container.Resolve<Qux>().Data, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestMethod()
        {
            PreInstall();

            Container.Bind<Qux>().FromSubContainerResolve()
                .ByNewPrefabMethod(FooPrefab, InstallFoo).AsCached();

            PostInstall();

            Assert.IsEqual(Container.Resolve<Qux>().Data, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestResourceInstaller()
        {
            PreInstall();

            Container.Bind<Qux>().FromSubContainerResolve()
                .ByNewPrefabResourceInstaller<FooInstaller>(FooPrefabResourcePath).AsCached();

            PostInstall();

            Assert.IsEqual(Container.Resolve<Qux>().Data, "asdf");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestResourceMethod()
        {
            PreInstall();

            Container.Bind<Qux>().FromSubContainerResolve()
                .ByNewPrefabResourceMethod(FooPrefabResourcePath, InstallFoo).AsCached();

            PostInstall();

            Assert.IsEqual(Container.Resolve<Qux>().Data, "asdf");
            yield break;
        }

        void InstallFoo(DiContainer subContainer)
        {
            subContainer.Bind<Qux>().AsSingle().WithArguments("asdf");
        }

        public class Qux
        {
            [Inject]
            public string Data;

            [Inject]
            public Foo Foo;
        }

        public class FooInstaller : Installer<FooInstaller>
        {
            public override void InstallBindings()
            {
                Container.Bind<Qux>().AsSingle().WithArguments("asdf");
            }
        }
    }
}

#endif
