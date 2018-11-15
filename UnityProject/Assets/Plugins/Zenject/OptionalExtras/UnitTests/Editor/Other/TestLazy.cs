using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings.Singletons
{
    [TestFixture]
    public class TestLazy : ZenjectUnitTestFixture
    {
        [Test]
        public void Test1()
        {
            Bar.InstanceCount = 0;

            Container.Bind<Bar>().AsSingle();
            Container.Bind<Foo>().AsSingle();

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(Bar.InstanceCount, 0);

            foo.DoIt();

            Assert.IsEqual(Bar.InstanceCount, 1);
        }

        [Test]
        public void TestOptional1()
        {
            Container.Bind<Bar>().AsSingle();
            Container.Bind<Qux>().AsSingle();

            Assert.IsNotNull(Container.Resolve<Qux>().Bar.Value);
        }

        [Test]
        public void TestOptional2()
        {
            Container.Bind<Qux>().AsSingle();

            Assert.IsNull(Container.Resolve<Qux>().Bar.Value);
        }

        [Test]
        public void TestOptional3()
        {
            Container.Bind<Gorp>().AsSingle();

            var gorp = Container.Resolve<Gorp>();
            object temp;
            Assert.Throws(() => temp = gorp.Bar.Value);
        }

        public class Bar
        {
            public static int InstanceCount;

            public Bar()
            {
                InstanceCount++;
            }

            public void DoIt()
            {
            }
        }

        public class Foo
        {
            readonly LazyInject<Bar> _bar;

            public Foo(LazyInject<Bar> bar)
            {
                _bar = bar;
            }

            public void DoIt()
            {
                _bar.Value.DoIt();
            }
        }

        public class Qux
        {
            [Inject(Optional = true)]
            public LazyInject<Bar> Bar;
        }

        public class Gorp
        {
            public LazyInject<Bar> Bar;
        }
    }
}

