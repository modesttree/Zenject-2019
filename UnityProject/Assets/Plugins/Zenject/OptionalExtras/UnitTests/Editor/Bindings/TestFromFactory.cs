using System.Linq;
using ModestTree;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromFactory : ZenjectUnitTestFixture
    {
        static Foo StaticFoo = new Foo();

        [Test]
        public void Test1()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached());

            Assert.IsEqual(Container.Resolve<Foo>(), StaticFoo);

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestOldVersion()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().FromFactory<FooFactory>();

            Assert.IsEqual(Container.Resolve<Foo>(), StaticFoo);

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestMoveIntoSubcontainers()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached()).MoveIntoDirectSubContainers();

            Assert.That(Container.AllContracts.Where(x => x.Type == typeof(IFactory<Foo>)).IsEmpty());

            Assert.IsNull(Container.TryResolve<Foo>());

            var subContainer = Container.CreateSubContainer();

            Assert.IsEqual(subContainer.Resolve<Foo>(), StaticFoo);

            Assert.That(subContainer.AllContracts.Where(x => x.Type == typeof(IFactory<Foo>)).Count() == 1);

            subContainer.Resolve<Foo>();
            subContainer.Resolve<Foo>();
            subContainer.Resolve<Foo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void Test2()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsTransient());

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();

            Assert.IsEqual(FooFactory.InstanceCount, 3);
        }

        [Test]
        public void Test3()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsTransient()).AsCached();

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestConcreteSingle()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<IFoo>().To<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached()).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<IFoo>(), StaticFoo);

            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestSelfAndConcreteSingle()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached()).AsSingle().NonLazy();
            Container.Bind<IFoo>().To<Foo>().FromResolve();

            Assert.IsEqual(Container.Resolve<IFoo>(), StaticFoo);
            Assert.IsEqual(Container.Resolve<Foo>(), StaticFoo);

            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestSelfCached()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached()).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), StaticFoo);

            Container.Resolve<Foo>();
            Container.Resolve<Foo>();
            Container.Resolve<Foo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        [Test]
        public void TestConcreteCached()
        {
            FooFactory.InstanceCount = 0;

            Container.Bind<IFoo>().To<Foo>().FromIFactory(b => b.To<FooFactory>().AsCached()).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<IFoo>(), StaticFoo);

            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();
            Container.Resolve<IFoo>();

            Assert.IsEqual(FooFactory.InstanceCount, 1);
        }

        class FooFactory : IFactory<Foo>
        {
            public static int InstanceCount;

            public FooFactory()
            {
                InstanceCount++;
            }

            public Foo Create()
            {
                return StaticFoo;
            }
        }

        interface IFoo
        {
        }

        class Foo : IFoo
        {
        }
    }
}

