using System.Linq;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromResolve : ZenjectUnitTestFixture
    {
        [Test]
        public void TestTransient()
        {
            var foo = new Foo();

            Container.BindInstance(foo);
            Container.Bind<IFoo>().To<Foo>().FromResolve();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), foo);
        }

        [Test]
        public void TestIdentifier()
        {
            var foo = new Foo();

            Container.Bind<Foo>().WithId("foo").FromInstance(foo);
            Container.Bind<IFoo>().To<Foo>().FromResolve("foo");

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.ResolveId<Foo>("foo"));
            Assert.IsEqual(Container.Resolve<IFoo>(), foo);
        }

        [Test]
        public void TestCached()
        {
            Container.Bind<Foo>().AsTransient();
            Container.Bind<IFoo>().To<Foo>().FromResolve().AsCached();

            Assert.IsNotEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestSingle()
        {
            var foo = new Foo();
            Container.Bind<Foo>().FromInstance(foo);
            Container.Bind<IFoo>().To<Foo>().FromResolve();

            Assert.IsEqual(Container.Resolve<IFoo>(), foo);
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestNoMatch()
        {
            Container.Bind<IFoo>().To<Foo>().FromResolve();

            Assert.Throws(() => Container.Resolve<IFoo>());
        }

        [Test]
        public void TestSingleFailure()
        {
            Container.Bind<Foo>().AsCached();
            Container.Bind<Foo>().AsCached();
            Container.Bind<IFoo>().To<Foo>().FromResolve();

            Assert.Throws(() => Container.Resolve<IFoo>());
        }

        [Test]
        public void TestInfiniteLoop()
        {
            Container.Bind<IFoo>().To<IFoo>().FromResolve().AsSingle();

            Assert.Throws(() => Container.Resolve<IFoo>());
        }

        [Test]
        public void TestResolveManyTransient()
        {
            Container.Bind<Foo>().AsTransient();
            Container.Bind<Foo>().FromInstance(new Foo());

            Container.Bind<IFoo>().To<Foo>().FromResolveAll();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
        }

        [Test]
        public void TestResolveManyTransient2()
        {
            Container.Bind<Foo>().AsTransient();
            Container.Bind<Foo>().FromInstance(new Foo());

            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().FromResolveAll();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 2);
        }

        [Test]
        public void TestResolveManyCached()
        {
            Container.Bind<Foo>().AsTransient();
            Container.Bind<Foo>().AsTransient();

            Container.Bind<IFoo>().To<Foo>().FromResolveAll().AsCached();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.That(Enumerable.SequenceEqual(Container.ResolveAll<IFoo>(), Container.ResolveAll<IFoo>()));
        }

        [Test]
        public void TestResolveManyCached2()
        {
            Container.Bind<Foo>().AsTransient();
            Container.Bind<Foo>().AsTransient();

            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().FromResolveAll().AsCached();

            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 2);
            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.That(Enumerable.SequenceEqual(Container.ResolveAll<IFoo>().Cast<object>(), Container.ResolveAll<IBar>().Cast<object>()));
        }

        [Test]
        public void TestResolveManyCached3()
        {
            Container.Bind<Foo>().AsTransient();
            Container.Bind<Foo>().AsTransient();

            Container.Bind<IFoo>().To<Foo>().FromResolveAll().AsCached();
            Container.Bind<IBar>().To<Foo>().FromResolveAll().AsCached();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 2);
            Assert.IsEqual(Container.ResolveAll<IBar>().Count, 2);
            Assert.That(!Enumerable.SequenceEqual(Container.ResolveAll<IFoo>().Cast<object>(), Container.ResolveAll<IBar>().Cast<object>()));
        }

        [Test]
        public void TestResolveSingleLocal()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();

            Container.Bind<Foo>().FromInstance(foo1);

            var subContainer = Container.CreateSubContainer();
            subContainer.Bind<Foo>().FromInstance(foo2);

            subContainer.Bind<IFoo>().To<Foo>().FromResolve();

            Assert.IsEqual(subContainer.Resolve<IFoo>(), foo2);
        }

        [Test]
        public void TestInjectSource1()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();

            Container.Bind<Foo>().FromInstance(foo1);

            var subContainer = Container.CreateSubContainer();
            subContainer.Bind<Foo>().FromInstance(foo2);

            subContainer.Bind<IFoo>().To<Foo>().FromResolve(null, InjectSources.Parent);

            Assert.IsEqual(subContainer.Resolve<IFoo>(), foo1);
        }

        [Test]
        public void TestInjectSource2()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            Container.Bind<Foo>().FromInstance(foo1);

            var subContainer = Container.CreateSubContainer();
            subContainer.Bind<Foo>().FromInstance(foo2);
            subContainer.Bind<Foo>().FromInstance(foo3);

            subContainer.Bind<IFoo>().To<Foo>().FromResolveAll(null, InjectSources.Local);

            Assert.Throws(() => subContainer.Resolve<IFoo>());
            Assert.That(Enumerable.SequenceEqual(subContainer.ResolveAll<IFoo>(), new [] { foo2, foo3, }));
        }

        interface IBar
        {
        }

        interface IFoo
        {
        }

        class Foo : IFoo, IBar
        {
        }
    }
}
