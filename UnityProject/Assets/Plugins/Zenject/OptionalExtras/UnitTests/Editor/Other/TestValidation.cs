using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestValidation
    {
        DiContainer Container
        {
            get; set;
        }

        [SetUp]
        public void Setup()
        {
            Container = new DiContainer(true);
            Container.Settings = new ZenjectSettings(
                ValidationErrorResponses.Throw, RootResolveMethods.All);
        }

        [Test]
        public void TestFailure()
        {
            Container.Bind<Bar>().AsSingle();

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestSuccess()
        {
            Container.Bind<Foo>().AsSingle();
            Container.Bind<Bar>().AsSingle();

            Container.ResolveRoots();
        }

        [Test]
        public void TestNumCalls()
        {
            Gorp.CallCount = 0;

            Container.BindInterfacesAndSelfTo<Gorp>().AsSingle();

            Container.ResolveRoots();

            Assert.IsEqual(Gorp.CallCount, 1);
        }

        [Test]
        public void TestFactoryFail()
        {
            Container.BindFactory<Bar, Bar.Factory>();

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestFactorySuccess()
        {
            Container.Bind<Foo>().AsSingle();
            Container.BindFactory<Bar, Bar.Factory>();

            Container.ResolveRoots();
        }

        [Test]
        public void TestSubContainerMethodSuccess()
        {
            Container.Bind<Qux>().FromSubContainerResolve().ByMethod(
                container =>
                {
                    container.Bind<Qux>().AsSingle();
                    container.Bind<Foo>().AsSingle();
                    container.Bind<Bar>().AsSingle();
                })
                .AsSingle();

            Container.ResolveRoots();
        }

        [Test]
        public void TestSubContainerMethodFailure()
        {
            Container.Bind<Qux>().FromSubContainerResolve().ByMethod(
                container =>
                {
                    container.Bind<Qux>().AsSingle();
                    container.Bind<Bar>().AsSingle();
                })
                .AsSingle();

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestSubContainerInstallerFailure()
        {
            Container.Bind<Qux>().FromSubContainerResolve().ByInstaller<QuxInstaller>().AsSingle();

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestLazyFail()
        {
            Container.Bind<Jaze>().AsSingle();

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestLazySuccess()
        {
            Container.Bind<Qux>().AsSingle();
            Container.Bind<Jaze>().AsSingle();

            Container.ResolveRoots();
        }

        [Test]
        public void TestMemoryPoolFailure()
        {
            Container.BindMemoryPool<Bar, Bar.Pool>();

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestMemoryPoolSuccess()
        {
            Container.Bind<Foo>().AsSingle();
            Container.BindMemoryPool<Bar, Bar.Pool>();

            Container.ResolveRoots();
        }

        [Test]
        public void TestCustomValidatable()
        {
            Container.BindInterfacesAndSelfTo<Loy>().AsSingle().NonLazy();

            Container.ResolveRoots();

            Assert.IsEqual(Container.Resolve<Loy>().CallCount, 1);
        }

        public class Loy : IValidatable, IInitializable, ITickable
        {
            public int CallCount
            {
                get; set;
            }

            public void Initialize()
            {
            }

            public void Tick()
            {
            }

            public void Validate()
            {
                CallCount++;
            }
        }

        public class Jaze
        {
            [Inject]
            public LazyInject<Qux> Qux;
        }

        public class QuxInstaller : Installer<QuxInstaller>
        {
            public override void InstallBindings()
            {
                Container.Bind<Qux>().AsSingle();
                Container.Bind<Bar>().AsSingle();
            }
        }

        public class Qux
        {
        }

        public class Bar
        {
            public Bar(Foo foo)
            {
            }

            public class Factory : PlaceholderFactory<Bar>
            {
            }

            public class Pool : MemoryPool<Bar>
            {
            }
        }

        public class Foo
        {
        }

        public interface IGorp
        {
        }

        public class Gorp : IGorp, IValidatable
        {
            public static int CallCount
            {
                get; set;
            }

            public void Validate()
            {
                CallCount++;
            }
        }
    }
}

