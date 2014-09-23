using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Exceptions;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestNestedContainer : TestWithContainer
    {
        public interface IFoo
        {
            int GetBar();
        }

        public class Foo : IFoo
        {
            public int GetBar()
            {
                return 0;
            }
        }

        public class Foo2 : IFoo
        {
            public int GetBar()
            {
                return 1;
            }
        }

        [Test]
        public void TestCase1()
        {
            var nestedContainer = new DiContainer();

            TestAssert.Throws<ZenjectResolveException>(() => nestedContainer.Resolve<IFoo>());
            TestAssert.Throws<ZenjectResolveException>(() => _container.Resolve<IFoo>());

            _container.Bind<IFoo>().ToSingle<Foo>();

            TestAssert.Throws<ZenjectResolveException>(() => nestedContainer.Resolve<IFoo>());
            TestAssert.AreEqual(_container.Resolve<IFoo>().GetBar(), 0);

            nestedContainer.FallbackProvider = new DiContainerProvider(_container);

            TestAssert.AreEqual(nestedContainer.Resolve<IFoo>().GetBar(), 0);
            TestAssert.AreEqual(_container.Resolve<IFoo>().GetBar(), 0);

            nestedContainer.Bind<IFoo>().ToSingle<Foo2>();

            TestAssert.AreEqual(nestedContainer.Resolve<IFoo>().GetBar(), 1);
            TestAssert.AreEqual(_container.Resolve<IFoo>().GetBar(), 0);
        }
    }
}
