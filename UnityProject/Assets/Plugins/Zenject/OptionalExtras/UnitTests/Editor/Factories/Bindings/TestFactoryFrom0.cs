using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFactoryFrom0 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSelf1()
        {
            Container.BindFactory<Foo, Foo.Factory>().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo.Factory>().Create());
        }

        [Test]
        public void TestSelf2()
        {
            Container.BindFactory<Foo, Foo.Factory>().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo.Factory>().Create());
        }

        [Test]
        public void TestSelf3()
        {
            Container.BindFactory<Foo, Foo.Factory>().FromNew().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo.Factory>().Create());
        }

        [Test]
        public void TestFactoryScopeDefault()
        {
            Container.BindFactory<Foo, Foo.Factory>();

            Assert.IsEqual(Container.Resolve<Foo.Factory>(), Container.Resolve<Foo.Factory>());
        }

        [Test]
        public void TestFactoryScopeTransient()
        {
            Container.BindFactory<Foo, Foo.Factory>().AsTransient();

            Assert.IsNotEqual(Container.Resolve<Foo.Factory>(), Container.Resolve<Foo.Factory>());
        }

        [Test]
        public void TestConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>().To<Foo>().NonLazy();

            Assert.IsNotNull(Container.Resolve<IFooFactory>().Create());

            Assert.That(Container.Resolve<IFooFactory>().Create() is Foo);
        }

        [Test]
        public void TestConcreteUntyped()
        {
            Container.BindFactory<IFoo, IFooFactory>().To(typeof(Foo)).NonLazy();

            Assert.IsNotNull(Container.Resolve<IFooFactory>().Create());

            Assert.That(Container.Resolve<IFooFactory>().Create() is Foo);
        }

        interface IFoo
        {
        }

        class IFooFactory : PlaceholderFactory<IFoo>
        {
        }

        class Foo : IFoo
        {
            public class Factory : PlaceholderFactory<Foo>
            {
            }
        }
    }
}
