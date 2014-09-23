using System;
using ModestTree.Zenject;
using NUnit.Framework;
using ModestTree.Zenject.Test;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Tests
{
    [TestFixture]
    public class TestMultipleInstallers : TestWithContainer
    {
        class Test0 : Installer
        {
            public static int Count;
            public override void InstallBindings()
            {
                Count++;
            }
        }

        class Test1 : Installer
        {
            public static int Count;
            public override void InstallBindings()
            {
                Container.Bind<IInstaller>().ToSingle<Test0>();
                Count++;
            }
        }

        class Test2 : Installer
        {
            public static int Count;
            public override void InstallBindings()
            {
                Container.Bind<IInstaller>().ToSingle<Test0>();
                Count++;
            }
        }

        public override void Setup()
        {
            base.Setup();

            // Reset counters since static state is not being reset by 'Unity Test Tools'.
            Test0.Count = 0;
            Test1.Count = 0;
            Test2.Count = 0;
        }

        [Test]
        public void Test()
        {
            _container.Bind<IInstaller>().ToSingle<Test1>();
            _container.Bind<IInstaller>().ToSingle<Test2>();

            _container.InstallInstallers();

            TestAssert.That(Test1.Count == 1);
            TestAssert.That(Test2.Count == 1);
            TestAssert.That(Test0.Count == 1);
        }
    }
}
