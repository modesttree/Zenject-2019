using System;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromMethod : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSingle()
        {
            var foo = new Foo();

            Container.Bind<Foo>().FromMethod(ctx => foo).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        public void TestTransient()
        {
            var foo = new Foo();

            Container.Bind<Foo>().FromMethod(ctx => foo).AsTransient().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        public void TestCached()
        {
            var foo = new Foo();

            Container.Bind<Foo>().FromMethod(ctx => foo).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        Foo CreateFoo(InjectContext ctx)
        {
            return new Foo();
        }

        [Test]
        public void TestTransient2()
        {
            int numCalls = 0;

            Func<InjectContext, Foo> method = ctx =>
            {
                numCalls++;
                return null;
            };

            Container.Bind<Foo>().FromMethod(method).AsTransient().NonLazy();
            Container.Bind<IFoo>().To<Foo>().FromMethod(method).AsTransient().NonLazy();

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(numCalls, 4);
        }

        [Test]
        public void TestCached2()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().FromMethod(ctx => new Foo()).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }
    }
}

