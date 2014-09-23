using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Misc;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestBaseClassPropertyInjection : TestWithContainer
    {
        class Test0
        {
        }

        class Test3
        {
        }

        class Test1 : Test3
        {
            [Inject] protected Test0 val = null;

            public Test0 GetVal()
            {
                return val;
            }
        }

        class Test2 : Test1
        {
        }

        [Test]
        public void TestCaseBaseClassPropertyInjection()
        {
            _container.Bind<Test0>().ToSingle();
            _container.Bind<Test2>().ToSingle();

            TestAssert.That(_container.ValidateResolve<Test2>().IsEmpty());
            var test1 = _container.Resolve<Test2>();

            TestAssert.That(test1.GetVal() != null);
        }
    }
}


