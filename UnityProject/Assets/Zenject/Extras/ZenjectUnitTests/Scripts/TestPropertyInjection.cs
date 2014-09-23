using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Misc;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestPropertyInjection : TestWithContainer
    {
        class Test1
        {
        }

        class Test2
        {
            [Inject]
            public Test1 val2 { get; private set; }

            [Inject]
            Test1 val4 { get; set; }

            public Test1 GetVal4()
            {
                return val4;
            }
        }

        [Test]
        public void TestPublicPrivate()
        {
            var test1 = new Test1();

            _container.Bind<Test2>().ToSingle();
            _container.Bind<Test1>().To(test1);

            TestAssert.That(_container.ValidateResolve<Test2>().IsEmpty());
            var test2 = _container.Resolve<Test2>();

            TestAssert.AreEqual(test2.val2, test1);
            TestAssert.AreEqual(test2.GetVal4(), test1);
        }

        [Test]
        public void TestCase2()
        {
        }
    }
}


