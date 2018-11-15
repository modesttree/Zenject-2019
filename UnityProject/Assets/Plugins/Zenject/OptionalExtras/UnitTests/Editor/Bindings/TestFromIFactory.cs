using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromIFactory : ZenjectUnitTestFixture
    {
        [Test]
        public void Test1()
        {
            Container.BindFactory<Foo, Foo.Factory>().WithId("foo1")
                .FromIFactory(x => x.To<FooFactory>().AsCached().WithArguments("asdf"));

            Container.BindFactory<Foo, Foo.Factory>().WithId("foo2")
                .FromIFactory(x => x.To<FooFactory>().AsCached().WithArguments("zxcv"));

            var factory1 = Container.ResolveId<Foo.Factory>("foo1");
            var factory2 = Container.ResolveId<Foo.Factory>("foo2");

            Assert.IsEqual(factory1.Create().Value, "asdf");
            Assert.IsEqual(factory2.Create().Value, "zxcv");
        }

        public class Foo
        {
            public Foo(string value)
            {
                Value = value;
            }

            public string Value
            {
                get; private set;
            }

            public class Factory : PlaceholderFactory<Foo>
            {
            }
        }

        public class FooFactory : IFactory<Foo>
        {
            readonly string _value;
            readonly DiContainer _container;

            public FooFactory(
                DiContainer container,
                string value)
            {
                _value = value;
                _container = container;
            }

            public Foo Create()
            {
                return _container.Instantiate<Foo>(new [] { _value });
            }
        }
    }
}
