using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFactoryFromSubContainerInstaller0 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>()
                .FromSubContainerResolve().ByInstaller<FooInstaller>().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo.Factory>().Create(), FooInstaller.Foo);
        }

        [Test]
        public void TestConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>()
                .To<Foo>().FromSubContainerResolve().ByInstaller<FooInstaller>().NonLazy();

            Assert.IsEqual(Container.Resolve<IFooFactory>().Create(), FooInstaller.Foo);
        }

        class FooInstaller : Installer<FooInstaller>
        {
            public static Foo Foo = new Foo();

            public override void InstallBindings()
            {
                Container.Bind<Foo>().FromInstance(Foo);
            }
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



