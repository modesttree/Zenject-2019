using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestBindCallbacks : ZenjectUnitTestFixture
    {
        public class Foo
        {
            [Inject]
            public int Value2
            {
                get; set;
            }

            public string Value
            {
                get; set;
            }

            public class Factory : PlaceholderFactory<Foo>
            {
            }

            public class Pool : MemoryPool<Foo>
            {
            }
        }

        [Test]
        public void Test1()
        {
            Container.BindInstance(5).WhenInjectedInto<Foo>();

            Container.Bind<Foo>().AsSingle().OnInstantiated<Foo>((ctx, f) =>
                {
                    Assert.IsEqual(f.Value2, 5);
                    f.Value = "asdf";
                });

            var foo = Container.Resolve<Foo>();

            Assert.IsEqual(foo.Value, "asdf");
        }

        [Test]
        public void TestFactory1()
        {
            Container.BindInstance(5).WhenInjectedInto<Foo>();

            Container.BindFactory<Foo, Foo.Factory>().OnInstantiated<Foo>((ctx, f) =>
                {
                    Assert.IsEqual(f.Value2, 5);
                    f.Value = "asdf";
                });

            var foo = Container.Resolve<Foo.Factory>().Create();

            Assert.IsEqual(foo.Value, "asdf");
        }

        [Test]
        public void TestMemoryPool1()
        {
            Container.BindInstance(5).WhenInjectedInto<Foo>();

            Container.BindMemoryPool<Foo, Foo.Pool>().OnInstantiated<Foo>((ctx, f) =>
                {
                    Assert.IsEqual(f.Value2, 5);
                    f.Value = "asdf";
                });

            var foo = Container.Resolve<Foo.Pool>().Spawn();

            Assert.IsEqual(foo.Value, "asdf");
        }
    }
}
