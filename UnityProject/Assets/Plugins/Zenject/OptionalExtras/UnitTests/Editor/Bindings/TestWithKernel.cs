using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestWithKernel : ZenjectUnitTestFixture
    {
        static int GlobalInitializeCount;

        public class Foo : IInitializable
        {
            public bool WasInitialized
            {
                get; private set;
            }

            public int InitializeCount
            {
                get; private set;
            }

            public void Initialize()
            {
                InitializeCount = ++GlobalInitializeCount;
                WasInitialized = true;
            }
        }

        public class FooFacade
        {
            [Inject]
            public Foo Foo
            {
                get; private set;
            }
        }

        public class FooInstaller : Installer<FooInstaller>
        {
            public override void InstallBindings()
            {
                InstallFoo(Container);
            }
        }

        static void InstallFoo(DiContainer subContainer)
        {
            subContainer.Bind<FooFacade>().AsSingle();
            subContainer.BindInterfacesAndSelfTo<Foo>().AsSingle();
        }

        [Test]
        public void TestByInstaller()
        {
            Container.Bind<FooFacade>().FromSubContainerResolve()
                .ByInstaller<FooInstaller>().WithKernel().AsSingle();

            ZenjectManagersInstaller.Install(Container);
            Container.ResolveRoots();

            var facade = Container.Resolve<FooFacade>();

            Assert.That(!facade.Foo.WasInitialized);
            Container.Resolve<InitializableManager>().Initialize();
            Assert.That(facade.Foo.WasInitialized);
        }

        [Test]
        public void TestByMethod()
        {
            Container.Bind<FooFacade>().FromSubContainerResolve()
                .ByMethod(InstallFoo).WithKernel().AsSingle();

            ZenjectManagersInstaller.Install(Container);
            Container.ResolveRoots();

            var facade = Container.Resolve<FooFacade>();

            Assert.That(!facade.Foo.WasInitialized);
            Container.Resolve<InitializableManager>().Initialize();
            Assert.That(facade.Foo.WasInitialized);
        }

        public class FooKernel : Kernel
        {
        }

        public class Bar : IInitializable
        {
            public int InitializeCount
            {
                get; private set;
            }

            public void Initialize()
            {
                InitializeCount = ++GlobalInitializeCount;
            }
        }

        [Test]
        public void TestByInstallerCustomOrder()
        {
            GlobalInitializeCount = 0;

            Container.BindInterfacesAndSelfTo<Bar>().AsSingle();
            Container.Bind<FooFacade>().FromSubContainerResolve()
                .ByInstaller<FooInstaller>().WithKernel<FooKernel>().AsSingle();

            ZenjectManagersInstaller.Install(Container);
            Container.ResolveRoots();

            var facade = Container.Resolve<FooFacade>();

            Assert.That(!facade.Foo.WasInitialized);
            Container.Resolve<InitializableManager>().Initialize();
            Assert.That(facade.Foo.WasInitialized);

            Assert.IsEqual(Container.Resolve<Bar>().InitializeCount, 1);
            Assert.IsEqual(facade.Foo.InitializeCount, 2);
        }

        [Test]
        public void TestByInstallerCustomOrder2()
        {
            GlobalInitializeCount = 0;

            Container.BindInterfacesAndSelfTo<Bar>().AsSingle();
            Container.Bind<FooFacade>().FromSubContainerResolve()
                .ByInstaller<FooInstaller>().WithKernel<FooKernel>().AsSingle();

            Container.BindExecutionOrder<FooKernel>(-1);

            ZenjectManagersInstaller.Install(Container);
            Container.ResolveRoots();

            var facade = Container.Resolve<FooFacade>();

            Assert.That(!facade.Foo.WasInitialized);
            Container.Resolve<InitializableManager>().Initialize();
            Assert.That(facade.Foo.WasInitialized);

            Assert.IsEqual(Container.Resolve<Bar>().InitializeCount, 2);
            Assert.IsEqual(facade.Foo.InitializeCount, 1);
        }
    }
}


