using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestPropertyInjection : ZenjectUnitTestFixture
    {
        class Test1
        {
        }

        class Test2
        {
            [Inject]
            public Test1 val2 { get; private set; }

            [Inject]
            Test1 val4 { get; set; }

            public Test1 GetVal4()
            {
                return val4;
            }
        }

        [Test]
        public void TestPublicPrivate()
        {
            var test1 = new Test1();

            Container.Bind<Test2>().AsSingle().NonLazy();
            Container.Bind<Test1>().FromInstance(test1).NonLazy();

            var test2 = Container.Resolve<Test2>();

            Assert.IsEqual(test2.val2, test1);
            Assert.IsEqual(test2.GetVal4(), test1);
        }

        [Test]
        public void TestCase2()
        {
        }
    }
}


