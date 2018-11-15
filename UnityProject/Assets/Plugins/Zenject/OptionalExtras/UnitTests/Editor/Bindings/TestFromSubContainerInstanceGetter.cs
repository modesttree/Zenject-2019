using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromSubContainerInstanceGetter : ZenjectUnitTestFixture
    {
        [Test]
        public void TestInstallerSelfSingle()
        {
            Container.Bind<Foo>().FromSubContainerResolve()
                .ByInstanceGetter(ctx => CreateFooSubContainer()).AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestInstallerSelfTransient()
        {
            Container.Bind<Foo>().FromSubContainerResolve()
                .ByInstanceGetter(ctx => CreateFooSubContainer()).AsTransient().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestInstallerSelfCached()
        {
            Container.Bind<Foo>().FromSubContainerResolve()
                .ByInstanceGetter(ctx => CreateFooSubContainer()).AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>().Bar);
        }

        [Test]
        public void TestInstallerSelfSingleMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve()
                .ByInstanceGetter(ctx => CreateFooSubContainer()).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>().Bar, Container.Resolve<Bar>());
        }

        [Test]
        public void TestInstallerSelfCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().FromSubContainerResolve()
                .ByInstanceGetter(ctx => CreateFooSubContainer()).AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestInstallerSelfSingleMultipleMatches()
        {
            Container.Bind<Qux>().FromSubContainerResolveAll()
                .ByInstanceGetter(ctx => CreateFooSubContainer()).AsSingle().NonLazy();

            Assert.IsEqual(Container.ResolveAll<Qux>().Count, 2);
        }

        [Test]
        public void TestInstallerSelfIdentifiersFails()
        {
            Container.Bind<Gorp>().FromSubContainerResolve()
                .ByInstanceGetter(ctx => CreateFooSubContainer()).AsSingle().NonLazy();

            Assert.Throws(() => Container.Resolve<Gorp>());
        }

        [Test]
        public void TestInstallerSelfIdentifiers()
        {
            Container.Bind<Gorp>().FromSubContainerResolve("gorp")
                .ByInstanceGetter(ctx => CreateFooSubContainer()).AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Gorp>());
        }

        public class Gorp
        {
        }

        public class Qux
        {
        }

        public class Bar
        {
        }

        public interface IFoo
        {
        }

        public class Foo : IFoo
        {
            public Foo(Bar bar)
            {
                Bar = bar;
            }

            public Bar Bar
            {
                get;
                private set;
            }
        }

        DiContainer CreateFooSubContainer()
        {
            var subContainer = new DiContainer();

            subContainer.Bind<Foo>().AsSingle();
            subContainer.Bind<Bar>().AsSingle();

            subContainer.Bind<Qux>().AsTransient();
            subContainer.Bind<Qux>().FromInstance(new Qux());

            subContainer.Bind<Gorp>().WithId("gorp").AsTransient();

            return subContainer;
        }
    }
}



