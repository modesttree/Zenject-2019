#if !(UNITY_WSA && ENABLE_DOTNET)

using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Convention.Two
{
    [TestFixture]
    public class TestConvention2
    {
        [Test]
        public void TestBindAllInterfacesSimple()
        {
            var container = new DiContainer();

            container.Bind(x => x.AllInterfaces()).To<Foo>().AsTransient();

            Assert.That(container.Resolve<IFoo>() is Foo);
            Assert.That(container.Resolve<IBar>() is Foo);
        }

        [Test]
        public void TestBindAllInterfaces2()
        {
            var container = new DiContainer();

            container.Bind(x => x.AllInterfaces())
                .To(x => x.AllNonAbstractClasses().InNamespace("Zenject.Tests.Convention.Two")).AsTransient();

            Assert.IsEqual(container.ResolveAll<IFoo>().Count, 2);
            Assert.IsEqual(container.ResolveAll<IBar>().Count, 2);
        }

        public interface IFoo
        {
        }

        public interface IBar
        {
        }

        public class Foo : IFoo, IBar
        {
        }

        public class Bar : IBar, IFoo
        {
        }
    }
}

#endif

