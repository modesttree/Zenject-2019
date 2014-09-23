using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Misc;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestFactoryTyped : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
        }

        class Test2
        {
            [Inject]
            public Test1 test1 = null;

            [Inject]
            public Test0 test0 = null;

            [Inject]
            public int value;

            // Test1 should be provided from container
            public class Factory : FactoryTyped<int, Test1, Test2>
            {
            }
        }

        [Test]
        public void Test()
        {
            _container.Bind<Test0>().ToSingle();
            _container.Bind<Test2.Factory>().ToSingle();

            var factory = _container.Resolve<Test2.Factory>();
            var test = factory.Create(5, new Test1());

            TestAssert.That(test.value == 5);
        }
    }
}

