#if UNITY_EDITOR

using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings.FromComponentInHierarchyGameObjectContext
{
    public class TestFromComponentInHierarchyGameObjectContext : ZenjectIntegrationTestFixture
    {
        GameObject FooPrefab
        {
            get
            {
                return FixtureUtil.GetPrefab("TestFromComponentInHierarchyGameObjectContext/Foo");
            }
        }

        [SetUp]
        public void SetUp()
        {
            new GameObject().AddComponent<Gorp>();
            new GameObject().AddComponent<Gorp>();
        }

        [UnityTest]
        public IEnumerator TestCorrectHierarchy()
        {
            PreInstall();

            Container.Bind<Foo>().FromSubContainerResolve()
                .ByNewContextPrefab(FooPrefab).AsSingle().NonLazy();

            PostInstall();

            var foo = Container.Resolve<Foo>();

            Assert.IsNotNull(foo.Gorp);
            Assert.IsEqual(foo.gameObject.GetComponentsInChildren<Gorp>().Single(), foo.Gorp);
            yield break;
        }
    }
}

#endif
