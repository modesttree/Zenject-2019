using System.Collections.Generic;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.BindFeatures
{
    [TestFixture]
    public class TestMultipleContractTypes3 : ZenjectUnitTestFixture
    {
        class Test0
        {
        }

        class Test3 : Test0
        {
        }

        class Test4 : Test0
        {
        }

        class Test2
        {
            public Test0 test;

            public Test2(Test0 test)
            {
                this.test = test;
            }
        }

        class Test1
        {
            public List<Test0> test;

            public Test1(List<Test0> test)
            {
                this.test = test;
            }
        }

        [Test]
        public void TestMultiBind2()
        {
            // Multi-binds should not map to single-binds
            Container.Bind<Test0>().To<Test3>().AsSingle().NonLazy();
            Container.Bind<Test0>().To<Test4>().AsSingle().NonLazy();
            Container.Bind<Test2>().AsSingle().NonLazy();

            Assert.Throws(() => Container.Resolve<Test2>());
        }
    }
}


