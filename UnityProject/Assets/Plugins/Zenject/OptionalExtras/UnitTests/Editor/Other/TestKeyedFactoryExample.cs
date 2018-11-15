using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestKeyedFactoryExample : ZenjectUnitTestFixture
    {
        [Test]
        public void Test1()
        {
            Container.BindFactory<Foo, Foo.Factory>().WithId("foo1")
                .FromSubContainerResolve().ByMethod(InstallFoo1);

            Container.BindFactory<Foo, Foo.Factory>().WithId("foo2")
                .FromSubContainerResolve().ByMethod(InstallFoo2);

            Container.Bind<Dictionary<string, IFactory<Foo>>>()
                .FromMethod(GetFooFactories).WhenInjectedInto<FooFactory>();

            Container.Bind<FooFactory>().AsSingle();

            var keyedFactory = Container.Resolve<FooFactory>();

            Assert.IsEqual(keyedFactory.Create("foo1").Number, 5);
            Assert.IsEqual(keyedFactory.Create("foo2").Number, 6);

            Assert.Throws(() => keyedFactory.Create("foo3"));
        }

        Dictionary<string, IFactory<Foo>> GetFooFactories(InjectContext ctx)
        {
            return ctx.Container.AllContracts.Where(
                x => x.Type == typeof(Foo.Factory))
                .ToDictionary(x => (string)x.Identifier, x => (IFactory<Foo>)ctx.Container.ResolveId<Foo.Factory>(x.Identifier));
        }

        void InstallFoo2(DiContainer subContainer)
        {
            subContainer.BindInstance(6);
            subContainer.Bind<Foo>().AsCached();
        }

        void InstallFoo1(DiContainer subContainer)
        {
            subContainer.BindInstance(5);
            subContainer.Bind<Foo>().AsCached();
        }

        public class FooFactory
        {
            readonly Dictionary<string, IFactory<Foo>> _subFactories;

            public FooFactory(
                Dictionary<string, IFactory<Foo>> subFactories)
            {
                _subFactories = subFactories;
            }

            public Foo Create(string key)
            {
                return _subFactories[key].Create();
            }
        }

        public class Foo
        {
            public Foo(int number)
            {
                Number = number;
            }

            public int Number
            {
                get; private set;
            }

            public class Factory : PlaceholderFactory<Foo>
            {
            }
        }
    }
}

