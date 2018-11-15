using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFactoryFromSubContainerMethod1 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSelf()
        {
            Container.BindFactory<string, Foo, Foo.Factory>()
                .FromSubContainerResolve().ByMethod(InstallFoo).NonLazy();

            Assert.IsEqual(Container.Resolve<Foo.Factory>().Create("asdf").Value, "asdf");
        }

        [Test]
        public void TestConcrete()
        {
            Container.BindFactory<string, IFoo, IFooFactory>().To<Foo>().FromSubContainerResolve().ByMethod(InstallFoo).NonLazy();

            Assert.IsEqual(Container.Resolve<IFooFactory>().Create("asdf").Value, "asdf");
        }

        void InstallFoo(DiContainer subContainer, string value)
        {
            subContainer.Bind<Foo>().AsSingle().WithArgumentsExplicit(
                InjectUtil.CreateArgListExplicit(value));
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



