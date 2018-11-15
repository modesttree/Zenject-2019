using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestConstructorInjectionOptional : ZenjectUnitTestFixture
    {
        class Test1
        {
        }

        class Test2
        {
            public Test1 val;

            public Test2(Test1 val = null)
            {
                this.val = val;
            }
        }

        class Test3
        {
            public Test1 val;

            public Test3(Test1 val)
            {
                this.val = val;
            }
        }

        [Test]
        public void TestCase1()
        {
            Container.Bind<Test2>().AsSingle().NonLazy();

            var test1 = Container.Resolve<Test2>();

            Assert.That(test1.val == null);
        }

        [Test]
        public void TestCase2()
        {
            Container.Bind<Test3>().AsSingle();

            Assert.Throws(() => Container.Instantiate<Test3>());
        }

        [Test]
        public void TestConstructByFactory()
        {
            Container.Bind<Test2>().AsSingle();

            var test1 = Container.Instantiate<Test2>();

            Assert.That(test1.val == null);
        }
    }
}


