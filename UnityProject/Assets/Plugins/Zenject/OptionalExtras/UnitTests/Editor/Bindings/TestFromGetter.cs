using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromGetter : ZenjectUnitTestFixture
    {
        [Test]
        public void TestTransient()
        {
            Container.Bind<Foo>().AsSingle();
            Container.Bind<Bar>().FromResolveGetter<Foo>(x => x.Bar);

            Assert.IsNotNull(Container.Resolve<Bar>());
            Assert.IsEqual(Container.Resolve<Bar>(), Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestSingleFailure()
        {
            Container.Bind<Foo>().AsCached();
            Container.Bind<Foo>().AsCached();
            Container.Bind<Bar>().FromResolveGetter<Foo>(x => x.Bar).AsSingle();

            Assert.Throws(() => Container.Resolve<Bar>());
        }

        [Test]
        public void TestMultiple()
        {
            Container.Bind<Foo>().AsCached();
            Container.Bind<Foo>().AsCached();
            Container.Bind<Bar>().FromResolveAllGetter<Foo>(x => x.Bar).AsSingle();

            Assert.IsEqual(Container.ResolveAll<Bar>().Count, 2);
        }

        [Test]
        public void TestInjectSource1()
        {
            Container.Bind<Foo>().AsCached();
            Container.Bind<Foo>().AsCached();

            var subContainer = Container.CreateSubContainer();
            subContainer.Bind<Foo>().AsCached();

            subContainer.Bind<Bar>().FromResolveAllGetter<Foo>(x => x.Bar);

            Assert.IsEqual(subContainer.ResolveAll<Bar>().Count, 3);
        }

        [Test]
        public void TestInjectSource2()
        {
            Container.Bind<Foo>().AsCached();
            Container.Bind<Foo>().AsCached();

            var subContainer = Container.CreateSubContainer();
            subContainer.Bind<Foo>().AsCached();

            subContainer.Bind<Bar>().FromResolveAllGetter<Foo>(null, x => x.Bar, InjectSources.Local);

            Assert.IsEqual(subContainer.ResolveAll<Bar>().Count, 1);
        }

        [Test]
        public void TestInjectSource3()
        {
            Container.Bind<Foo>().AsCached();
            Container.Bind<Foo>().AsCached();

            var subContainer = Container.CreateSubContainer();
            subContainer.Bind<Foo>().AsCached();

            subContainer.Bind<Bar>().FromResolveGetter<Foo>(null, x => x.Bar);

            Assert.IsNotNull(subContainer.Resolve<Bar>());
        }

        [Test]
        public void TestInjectSource4()
        {
            Container.Bind<Foo>().AsCached();

            var subContainer = Container.CreateSubContainer();
            subContainer.Bind<Foo>().AsCached();
            subContainer.Bind<Foo>().AsCached();

            subContainer.Bind<Bar>().FromResolveGetter<Foo>(null, x => x.Bar, InjectSources.Parent);

            Assert.IsEqual(subContainer.ResolveAll<Bar>().Count, 1);
        }

        interface IBar
        {
        }

        class Bar : IBar
        {
        }

        class Foo
        {
            public Foo()
            {
                Bar = new Bar();
            }

            public Bar Bar
            {
                get; private set;
            }
        }
    }
}

