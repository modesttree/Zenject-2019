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
    public class TestParameters : TestWithContainer
    {
        private class Test1
        {
            public int f1;
            public int f2;

            public Test1(int f1, int f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
        }

        [Test]
        public void TestExtraParametersSameType()
        {
            var factory1 = new Factory<Test1>(_container);
            var test1 = factory1.Create(5, 10);

            TestAssert.That(test1 != null);
            TestAssert.That(test1.f1 == 5 && test1.f2 == 10);

            var factory2 = new Factory<Test1>(_container);
            var test2 = factory2.Create(10, 5);

            TestAssert.That(test2 != null);
            TestAssert.That(test2.f1 == 10 && test2.f2 == 5);
        }

        [Test]
        public void TestMissingParameterThrows()
        {
            _container.Bind<Test1>().ToTransient();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test1>(); });

            TestAssert.That(_container.ValidateResolve<Test1>().Any());
        }
    }
}


