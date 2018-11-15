using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings.Singletons
{
    [TestFixture]
    public class TestAsSingle : ZenjectUnitTestFixture
    {
        [Test]
        public void TestAsSingleThrows()
        {
            Container.Bind<Foo>().AsSingle();
            Container.Bind<Foo>().AsSingle();

            Assert.Throws(() => Container.FlushBindings());
        }

        [Test]
        public void TestAsSingleAndTransientThrows()
        {
            Container.Bind<Foo>().AsSingle();
            Container.Bind<Foo>().AsTransient();

            Assert.Throws(() => Container.FlushBindings());
        }

        [Test]
        public void TestAsSingleAndResolveNoThrow()
        {
            Container.Bind<Foo>().AsSingle();
            Container.Bind<IFoo>().To<Foo>().FromResolve();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestToSingleMethod1()
        {
            Container.Bind<Foo>().AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromMethod(container => new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromInstance(new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached()).AsSingle();
                    Container.FlushBindings();
                });
        }

        [Test]
        public void TestToSingleMethod()
        {
            Container.Bind<Foo>().FromMethod(container => new Foo()).AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromInstance(new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached()).AsSingle();
                    Container.FlushBindings();
                });
        }

        [Test]
        public void TestToSingleInstance()
        {
            Container.Bind<Foo>().FromInstance(new Foo()).AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromMethod(container => new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached()).AsSingle();
                    Container.FlushBindings();
                });
        }

        [Test]
        public void TestToSingleFactory()
        {
            Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached()).AsSingle();

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromMethod(container => new Foo()).AsSingle();
                    Container.FlushBindings();
                });

            Assert.Throws(() =>
                {
                    Container.Bind<Foo>().FromInstance(new Foo()).AsSingle();
                    Container.FlushBindings();
                });
        }

        class Bar
        {
            public Foo GetFoo()
            {
                return new Foo();
            }
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }

        class FooFactory : IFactory<Foo>
        {
            public Foo Create()
            {
                return new Foo();
            }
        }
    }
}


