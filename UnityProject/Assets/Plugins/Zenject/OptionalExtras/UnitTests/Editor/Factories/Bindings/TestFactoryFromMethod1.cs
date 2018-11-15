using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFactoryFromMethod1 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSelf()
        {
            Container.BindFactory<string, Foo, Foo.Factory>().FromMethod((c, value) => new Foo(value)).NonLazy();

            Assert.IsEqual(Container.Resolve<Foo.Factory>().Create("asdf").Value, "asdf");
        }

        [Test]
        public void TestConcrete()
        {
            Container.BindFactory<string, IFoo, IFooFactory>().FromMethod((c, value) => new Foo(value)).NonLazy();

            Assert.IsEqual(Container.Resolve<IFooFactory>().Create("asdf").Value, "asdf");
        }

        interface IFoo
        {
            string Value
            {
                get;
            }

        }

        class IFooFactory : PlaceholderFactory<string, IFoo>
        {
        }

        class Foo : IFoo
        {
            public Foo(string value)
            {
                Value = value;
            }

            public string Value
            {
                get;
                private set;
            }

            public class Factory : PlaceholderFactory<string, Foo>
            {
            }
        }
    }
}


