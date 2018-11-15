using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Conditions
{
    [TestFixture]
    public class TestConditionsIdentifier : ZenjectUnitTestFixture
    {
        class Test0
        {
        }

        class Test1
        {
            public Test1(
                [Inject(Id ="foo")]
                Test0 name1)
            {
            }
        }

        class Test2
        {
            [Inject(Id ="foo")]
            public Test0 name2;
        }

        [Test]
        public void TestUnspecifiedNameConstructorInjection()
        {
            Container.Bind<Test1>().AsTransient().NonLazy();
            Container.Bind<Test0>().AsTransient().NonLazy();

            Assert.Throws(
                delegate { Container.Resolve<Test1>(); });
        }

        [Test]
        public void TestUnspecifiedNameFieldInjection()
        {
            Container.Bind<Test1>().AsTransient().NonLazy();
            Container.Bind<Test2>().AsTransient().NonLazy();

            Container.Bind<Test0>().AsTransient().NonLazy();

            Assert.Throws(
                delegate { Container.Resolve<Test2>(); });
        }

        [Test]
        public void TestSuccessConstructorInjectionString()
        {
            Container.Bind<Test1>().AsTransient().NonLazy();
            Container.Bind<Test2>().AsTransient().NonLazy();

            Container.Bind<Test0>().FromInstance(new Test0()).NonLazy();
            Container.Bind<Test0>().WithId("foo").FromInstance(new Test0()).NonLazy();

            // Should not throw exceptions
            Container.Resolve<Test1>();

            Assert.IsNotNull(Container.Resolve<Test1>());
        }

        [Test]
        public void TestSuccessFieldInjectionString()
        {
            Container.Bind<Test1>().AsTransient().NonLazy();
            Container.Bind<Test2>().AsTransient().NonLazy();

            Container.Bind<Test0>().FromInstance(new Test0()).NonLazy();
            Container.Bind<Test0>().WithId("foo").FromInstance(new Test0()).NonLazy();

            Assert.IsNotNull(Container.Resolve<Test2>());
        }

        class Test3
        {
            public Test3(
                [Inject(Id ="TestValue2")]
                Test0 test0)
            {
            }
        }

        class Test4
        {

        }

        [Test]
        public void TestFailConstructorInjectionEnum()
        {
            Container.Bind<Test1>().AsTransient().NonLazy();
            Container.Bind<Test2>().AsTransient().NonLazy();
            Container.Bind<Test3>().AsTransient().NonLazy();

            Container.Bind<Test0>().FromInstance(new Test0()).NonLazy();
            Container.Bind<Test0>().WithId("TestValue1").FromInstance(new Test0()).NonLazy();

            Assert.Throws(
                delegate { Container.Resolve<Test3>(); });
        }

        [Test]
        public void TestSuccessConstructorInjectionEnum()
        {
            Container.Bind<Test3>().AsTransient().NonLazy();

            Container.Bind<Test0>().FromInstance(new Test0()).NonLazy();
            Container.Bind<Test0>().WithId("TestValue2").FromInstance(new Test0()).NonLazy();

            // No exceptions
            Container.Resolve<Test3>();

            Assert.IsNotNull(Container.Resolve<Test3>());
        }

        [Test]
        public void TestFailFieldInjectionEnum()
        {
            Container.Bind<Test1>().AsTransient().NonLazy();
            Container.Bind<Test2>().AsTransient().NonLazy();
            Container.Bind<Test3>().AsTransient().NonLazy();

            Container.Bind<Test0>().FromInstance(new Test0()).NonLazy();
            Container.Bind<Test0>().WithId("TestValue1").FromInstance(new Test0()).NonLazy();

            Assert.Throws(
                delegate { Container.Resolve<Test3>(); });
        }

        [Test]
        public void TestSuccessFieldInjectionEnum()
        {
            Container.Bind<Test4>().AsTransient().NonLazy();

            Container.Bind<Test0>().FromInstance(new Test0()).NonLazy();
            Container.Bind<Test0>().WithId("TestValue3").FromInstance(new Test0()).NonLazy();

            Assert.IsNotNull(Container.Resolve<Test4>());
        }
    }
}
