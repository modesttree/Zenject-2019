using System;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestFacadeSubContainer
    {
        static int NumInstalls;

        [Test]
        public void Test1()
        {
            NumInstalls = 0;
            InitTest.WasRun = false;
            TickTest.WasRun = false;
            DisposeTest.WasRun = false;

            var container = new DiContainer();

            container.Bind(typeof(TickableManager), typeof(InitializableManager), typeof(DisposableManager))
                .ToSelf().AsSingle().CopyIntoAllSubContainers();

            // This is how you add ITickables / etc. within sub containers
            container.BindInterfacesAndSelfTo<FooKernel>()
                .FromSubContainerResolve().ByMethod(InstallFoo).AsSingle();

            var tickManager = container.Resolve<TickableManager>();
            var initManager = container.Resolve<InitializableManager>();
            var disposeManager = container.Resolve<DisposableManager>();

            Assert.That(!InitTest.WasRun);
            Assert.That(!TickTest.WasRun);
            Assert.That(!DisposeTest.WasRun);

            initManager.Initialize();
            tickManager.Update();
            disposeManager.Dispose();

            Assert.That(InitTest.WasRun);
            Assert.That(TickTest.WasRun);
            Assert.That(DisposeTest.WasRun);
        }

        public void InstallFoo(DiContainer subContainer)
        {
            NumInstalls++;

            subContainer.Bind<FooKernel>().AsSingle();

            subContainer.Bind<IInitializable>().To<InitTest>().AsSingle();
            subContainer.Bind<ITickable>().To<TickTest>().AsSingle();
            subContainer.Bind<IDisposable>().To<DisposeTest>().AsSingle();
        }

        public class FooKernel : Kernel
        {
        }

        public class InitTest : IInitializable
        {
            public static bool WasRun;

            public void Initialize()
            {
                WasRun = true;
            }
        }

        public class TickTest : ITickable
        {
            public static bool WasRun;

            public void Tick()
            {
                WasRun = true;
            }
        }

        public class DisposeTest : IDisposable
        {
            public static bool WasRun;

            public void Dispose()
            {
                WasRun = true;
            }
        }
    }
}


