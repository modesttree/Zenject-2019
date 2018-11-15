using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Conditions
{
    [TestFixture]
    public class TestIdentifierTypes : ZenjectUnitTestFixture
    {
        class Foo
        {
        }

        enum Things
        {
            Thing1,
            Thing2
        }

        class Test0
        {
            public Test0(
                [Inject(Id = "asdf")]
                Foo foo)
            {
            }
        }

        [Test]
        public void TestStringIdentifiers1()
        {
            Container.Bind<Foo>().AsTransient();
            Container.Bind<Test0>().AsTransient();

            Assert.Throws(() => Container.Resolve<Test0>());
        }

        [Test]
        public void TestStringIdentifiers2()
        {
            Container.Bind<Foo>().WithId("asdf").AsTransient();
            Container.Bind<Test0>().AsTransient();

            Assert.IsNotNull(Container.Resolve<Test0>());
        }

        class Test1
        {
            public Test1(
                [Inject(Id = 5)]
                Foo foo)
            {
            }
        }

        [Test]
        public void TestIntIdentifiers1()
        {
            Container.Bind<Foo>().AsTransient();
            Container.Bind<Test1>().AsTransient();

            Assert.Throws(() => Container.Resolve<Test1>());
        }

        [Test]
        public void TestIntIdentifiers2()
        {
            Container.Bind<Foo>().WithId(4).AsTransient();
            Container.Bind<Test1>().AsTransient();

            Assert.Throws(() => Container.Resolve<Test1>());
        }

        [Test]
        public void TestIntIdentifiers3()
        {
            Container.Bind<Foo>().WithId(5).AsTransient();
            Container.Bind<Test1>().AsTransient();

            Assert.IsNotNull(Container.Resolve<Test1>());
        }

        class Test2
        {
            public Test2(
                [Inject(Id = Things.Thing1)]
                Foo foo)
            {
            }
        }

        [Test]
        public void TestEnumIdentifiers1()
        {
            Container.Bind<Foo>().AsTransient();
            Container.Bind<Test2>().AsTransient();

            Assert.Throws(() => Container.Resolve<Test2>());
        }

        [Test]
        public void TestEnumIdentifiers2()
        {
            Container.Bind<Foo>().WithId(Things.Thing2).AsTransient();
            Container.Bind<Test2>().AsTransient();

            Assert.Throws(() => Container.Resolve<Test2>());
        }

        [Test]
        public void TestEnumIdentifiers3()
        {
            Container.Bind<Foo>().WithId(Things.Thing1).AsTransient();
            Container.Bind<Test2>().AsTransient();

            Assert.IsNotNull(Container.Resolve<Test2>());
        }
    }
}

