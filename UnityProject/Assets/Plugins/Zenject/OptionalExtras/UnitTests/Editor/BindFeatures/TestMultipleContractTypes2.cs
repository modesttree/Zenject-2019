using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.BindFeatures
{
    [TestFixture]
    public class TestMultipleContractTypes2 : ZenjectUnitTestFixture
    {
        public class Bar
        {
        }

        public interface IFoo
        {
        }

        public interface IQux
        {
        }

        public class Foo : IQux, IFoo
        {
        }

        [Test]
        public void Test1()
        {
            var types = new[]
            {
                typeof(Bar),
                typeof(Foo)
            };

            Container.Bind(types).AsSingle().NonLazy();

            Container.Resolve<Bar>();
            Container.Resolve<Foo>();
        }

        [Test]
        public void TestInterfaces()
        {
            Container.Bind<IFoo>().AsSingle().NonLazy();

            Assert.Throws(() => Container.FlushBindings());
        }

        [Test]
        public void TestAllInterfacesMistake()
        {
            Container.BindInterfacesTo<Foo>();

            // Should require setting scope
            Assert.Throws(() => Container.FlushBindings());
        }

        [Test]
        public void TestAllInterfaces()
        {
            Container.BindInterfacesTo<Foo>().AsSingle().NonLazy();

            Assert.IsNull(Container.TryResolve<Foo>());

            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsNotNull(Container.Resolve<IQux>());
        }

        [Test]
        public void TestAllInterfacesAndSelf()
        {
            Container.BindInterfacesAndSelfTo<Foo>().AsSingle().NonLazy();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsNotNull(Container.Resolve<IQux>());
        }

        [Test]
        public void TestAllInterfacesAndSelfMistake()
        {
            Container.BindInterfacesAndSelfTo<Foo>();

            // Should require setting scope
            Assert.Throws(() => Container.FlushBindings());
        }
    }
}



