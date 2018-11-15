#if UNITY_EDITOR

using System.Collections;
using ModestTree;
using UnityEngine.TestTools;
using Zenject.Tests.Installers.Installers;

namespace Zenject.Tests.Installers
{
    public class TestInstallers : ZenjectIntegrationTestFixture
    {
        [UnityTest]
        public IEnumerator TestZeroArgs()
        {
            PreInstall();
            FooInstaller.Install(Container);

            PostInstall();

            FixtureUtil.AssertResolveCount<Foo>(Container, 1);
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOneArg()
        {
            PreInstall();
            BarInstaller.Install(Container, "blurg");

            PostInstall();

            Assert.IsEqual(Container.Resolve<string>(), "blurg");
            yield break;
        }

        [UnityTest]
        public IEnumerator TestThreeArgs()
        {
            PreInstall();
            QuxInstaller.Install(Container, "blurg", 2.0f, 1);

            PostInstall();

            Assert.IsEqual(Container.Resolve<string>(), "blurg");
            yield break;
        }
    }
}

#endif
