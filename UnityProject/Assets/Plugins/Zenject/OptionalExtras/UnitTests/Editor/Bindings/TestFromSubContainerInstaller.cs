using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromSubContainerInstaller : ZenjectUnitTestFixture
    {
        [Test]
        public void TestInstallerSelfSingle()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByInstaller<FooInstaller>().AsSingle().NonLazy();

            var foo = Container.Resolve<Foo>();
            Assert.IsNotNull(foo.Bar);
            Assert.IsEqual(foo, Container.Resolve<Foo>());
        }

        [Test]
        public void TestInstallerSelfTransient()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByInstaller<FooInstaller>().AsTransient().NonLazy();

            var foo = Container.Resolve<Foo>();
            Assert.IsNotNull(foo.Bar);
            Assert.IsNotEqual(foo, Container.Resolve<Foo>());
        }

        [Test]
        public void TestInstallerSelfCached()
        {
            Container.Bind<Foo>().FromSubContainerResolve().ByInstaller<FooInstaller>().AsSingle().NonLazy();

            var foo = Container.Resolve<Foo>();
            Assert.IsNotNull(foo.Bar);
            Assert.IsEqual(foo, Container.Resolve<Foo>());
        }

        [Test]
        public void TestInstallerSelfSingleMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(Bar)).FromSubContainerResolve().ByInstaller<FooInstaller>().AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>().Bar, Container.Resolve<Bar>());
        }

        [Test]
        public void TestInstallerSelfCachedMultipleContracts()
        {
            Container.Bind(typeof(Foo), typeof(IFoo)).To<Foo>().FromSubContainerResolve().ByInstaller<FooInstaller>().AsSingle().NonLazy();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestInstallerSelfSingleMultipleMatches()
        {
            Container.Bind<Qux>().FromSubContainerResolveAll().ByInstaller<FooInstaller>().AsSingle().NonLazy();

            Assert.IsEqual(Container.ResolveAll<Qux>().Count, 2);
        }

        [Test]
        public void TestInstallerSelfIdentifiersFails()
        {
            Container.Bind<Gorp>().FromSubContainerResolve().ByInstaller<FooInstaller>().AsSingle().NonLazy();

            Assert.Throws(() => Container.Resolve<Gorp>());
        }

        [Test]
        public void TestInstallerSelfIdentifiers()
        {
            Container.Bind<Gorp>().FromSubContainerResolve("gorp").ByInstaller<FooInstaller>().AsSingle().NonLazy();

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

        class FooInstaller : Installer<FooInstaller>
        {
            public override void InstallBindings()
            {
                Container.Bind<Foo>().AsSingle();
                Container.Bind<Bar>().AsSingle();

                Container.Bind<Qux>().AsTransient();
                Container.Bind<Qux>().FromInstance(new Qux());

                Container.Bind<Gorp>().WithId("gorp").AsTransient();
            }
        }
    }
}

