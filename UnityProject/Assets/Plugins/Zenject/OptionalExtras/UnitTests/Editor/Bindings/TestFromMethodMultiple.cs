using System;
using System.Collections.Generic;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromMethodMultiple : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSingle()
        {
            var foo = new Foo();

            Container.Bind<Foo>().FromMethodMultiple(ctx => new[] { foo }).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        public void TestSingle5()
        {
            // This is weird but consistent with how AsSingle is interpreted for other From types
            // like FromSubcontainerResolve, FromComponentInPrefab, etc.
            // The 'single' is really refering to the fact that it's a single resolve handler, not a
            // single instance
            Container.Bind<Foo>().FromMethodMultiple(ctx => new[] { new Foo(), new Foo() }).AsSingle().NonLazy();

            Assert.IsEqual(Container.ResolveAll<Foo>().Count, 2);
        }

        [Test]
        public void TestMisc()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();

            Container.Bind<Foo>().FromMethodMultiple(ctx => new[] { foo1, foo2 });

            var foos = Container.ResolveAll<Foo>();
            Assert.IsEqual(foos[0], foo1);
            Assert.IsEqual(foos[1], foo2);
        }

        [Test]
        public void TestMisc2()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();
            var foo4 = new Foo();

            Container.Bind<Foo>().FromMethodMultiple(ctx => new[] { foo1, foo2 });
            Container.Bind<Foo>().FromMethodMultiple(ctx => new[] { foo3, foo4 });

            var foos = Container.ResolveAll<Foo>();

            Assert.IsEqual(foos[0], foo1);
            Assert.IsEqual(foos[1], foo2);
            Assert.IsEqual(foos[2], foo3);
            Assert.IsEqual(foos[3], foo4);
        }

        [Test]
        public void TestTransient()
        {
            var foo = new Foo();

            Container.Bind<Foo>().FromMethodMultiple(ctx => new[] { foo }).AsTransient().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        [Test]
        public void TestCached()
        {
            var foo = new Foo();

            Container.Bind<Foo>().FromMethodMultiple(ctx => new[] { foo }).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), foo);
        }

        IEnumerable<Foo> CreateFoos(InjectContext ctx)
        {
            yield return new Foo();
        }

        [Test]
        public void TestTransient2()
        {
            int numCalls = 0;

            Func<InjectContext, IEnumerable<Foo>> method = ctx =>
            {
                numCalls++;
                return new[] { new Foo() };
            };

            Container.Bind<Foo>().FromMethodMultiple(method).AsTransient().NonLazy();
            Container.Bind<IFoo>().To<Foo>().FromMethodMultiple(method).AsTransient().NonLazy();

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(numCalls, 4);
        }

        [Test]
        public void TestCached2()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().FromMethodMultiple(ctx => new[] { new Foo() }).AsSingle().NonLazy();

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

