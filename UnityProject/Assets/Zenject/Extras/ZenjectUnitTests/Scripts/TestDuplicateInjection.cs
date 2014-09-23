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
    public class TestDuplicateInjection : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test1
        {
            public Test1(Test0 test1)
            {
            }
        }

        [Test]
        public void TestCaseDuplicateInjection()
        {
            _container.Bind<Test0>().ToSingle();
            _container.Bind<Test0>().ToSingle();

            _container.Bind<Test1>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test1>(); });

            TestAssert.That(_container.ValidateResolve<Test1>().Any());
        }
    }
}


