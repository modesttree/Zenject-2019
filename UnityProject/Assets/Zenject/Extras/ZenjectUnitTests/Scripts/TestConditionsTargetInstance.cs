using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Misc;
using NUnit.Framework;
using TestAssert = NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestConditionsTargetInstance : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
            [Inject]
            public Test0 test0 = null;
        }

        Test1 _test1;

        public override void Setup()
        {
            base.Setup();

            _test1 = new Test1();
            _container.Bind<Test0>().ToSingle().When(r => r.EnclosingInstance == _test1);
            _container.Bind<Test1>().To(_test1);
        }

        [Test]
        public void TestTargetConditionError()
        {
            FieldsInjecter.Inject(_container, _test1);

            TestAssert.That(_test1.test0 != null);
        }
    }
}
