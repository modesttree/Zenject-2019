using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Exceptions;
using ModestTree.Zenject.Api.Misc;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;
using System.Linq;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestCircularDependencies : TestWithContainer
    {
        class Test1
        {
            [Inject]
            public Test2 test = null;
        }

        class Test2
        {
            [Inject]
            public Test2 test = null;
        }

        [Test]
        [ExpectedException]
        public void Test()
        {
            _container.Bind<Test1>().ToSingle();
            _container.Bind<Test2>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test2>(); });

            TestAssert.That(_container.ValidateResolve<Test2>().Any());
        }
    }
}


