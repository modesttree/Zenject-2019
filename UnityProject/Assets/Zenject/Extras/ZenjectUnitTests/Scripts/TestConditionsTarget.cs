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
    public class TestConditionsTarget : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test1
        {
            public Test1(Test0 test)
            {
            }
        }

        private class Test2
        {
            public Test2(Test0 test)
            {
            }
        }

        public override void Setup()
        {
            base.Setup();
            _container.Bind<Test0>().ToSingle().When(r => r.EnclosingType == typeof(Test2));
        }

        [Test]
        public void TestTargetConditionError()
        {
            _container.Bind<Test1>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test1>(); });

            TestAssert.That(_container.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestTargetConditionSuccess()
        {
            _container.Bind<Test2>().ToSingle();
            TestAssert.That(_container.ValidateResolve<Test2>().IsEmpty());
            var test2 = _container.Resolve<Test2>();

            TestAssert.That(test2 != null);
        }
    }
}


