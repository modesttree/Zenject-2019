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
    public class TestMultiBind : TestWithContainer
    {
        class Test1
        {
        }

        class Test2 : Test1
        {
        }

        class Test3 : Test1
        {
        }

        class TestImpl1
        {
            public List<Test1> tests;

            public TestImpl1(List<Test1> tests)
            {
                this.tests = tests;
            }
        }

        class TestImpl2
        {
            [Inject]
            public List<Test1> tests = null;
        }

        [Test]
        public void TestMultiBind1()
        {
            _container.Bind<Test1>().ToSingle<Test2>();
            _container.Bind<Test1>().ToSingle<Test3>();
            _container.Bind<TestImpl1>().ToSingle();

            TestAssert.That(_container.ValidateResolve<TestImpl1>().IsEmpty());
            var test1 = _container.Resolve<TestImpl1>();

            TestAssert.That(test1.tests.Count == 2);
        }

        [Test]
        public void TestMultiBind2()
        {
            _container.Bind<TestImpl1>().ToSingle();

            // optional list dependencies should be declared as optional
            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<TestImpl1>(); });

            TestAssert.That(_container.ValidateResolve<TestImpl1>().Any());
        }

        [Test]
        public void TestMultiBind2Validate()
        {
            _container.Bind<TestImpl1>().ToSingle();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<TestImpl1>(); });

            TestAssert.That(_container.ValidateResolve<Test2>().Any());
        }

        [Test]
        public void TestMultiBindListInjection()
        {
            _container.Bind<Test1>().ToSingle<Test2>();
            _container.Bind<Test1>().ToSingle<Test3>();
            _container.Bind<TestImpl2>().ToSingle();

            TestAssert.That(_container.ValidateResolve<TestImpl2>().IsEmpty());
            var test = _container.Resolve<TestImpl2>();
            TestAssert.That(test.tests.Count == 2);
        }
    }
}


