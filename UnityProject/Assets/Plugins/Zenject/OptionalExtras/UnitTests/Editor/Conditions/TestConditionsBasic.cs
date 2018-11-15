using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Conditions
{
    [TestFixture]
    public class TestConditionsBasic : ZenjectUnitTestFixture
    {
        public interface IFoo
        {
        }

        class Foo1 : IFoo
        {
        }

        class Foo2 : IFoo
        {
        }

        class Bar1
        {
            public IFoo Foo;

            public Bar1(IFoo foo)
            {
                Foo = foo;
            }
        }

        class Bar2
        {
            public IFoo Foo;

            public Bar2(IFoo foo)
            {
                Foo = foo;
            }
        }

        [Test]
        public void Test1()
        {
            Container.Bind<Bar1>().AsSingle().NonLazy();
            Container.Bind<Bar2>().AsSingle().NonLazy();
            Container.Bind<IFoo>().To<Foo1>().AsSingle().NonLazy();
            Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenInjectedInto<Bar2>().NonLazy();

            Assert.IsNotEqual(
                Container.Resolve<Bar1>().Foo, Container.Resolve<Bar2>().Foo);
        }

        [Test]
        public void Test2()
        {
            Container.Bind<Bar1>().AsSingle().NonLazy();
            Container.Bind<Bar2>().AsSingle().NonLazy();
            Container.Bind<IFoo>().To<Foo1>().AsSingle().NonLazy();
            Container.Bind<IFoo>().To<Foo2>().AsSingle().WhenNotInjectedInto<Bar1>().NonLazy();

            Assert.IsNotEqual(
                Container.Resolve<Bar1>().Foo, Container.Resolve<Bar2>().Foo);
        }
    }
}



