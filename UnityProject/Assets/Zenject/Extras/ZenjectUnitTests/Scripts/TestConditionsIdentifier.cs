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
    public class TestConditionsIdentifier : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
            public Test1(
                [Inject("foo")]
                Test0 name1)
            {
            }
        }

        class Test2
        {
            [Inject("foo")]
            Test0 name2;
        }

        public override void Setup()
        {
            base.Setup();

            _container.Bind<Test1>().ToTransient();
            _container.Bind<Test2>().ToTransient();
            _container.Bind<Test3>().ToTransient();
            _container.Bind<Test4>().ToTransient();
        }

        [Test]
        public void TestUnspecifiedNameConstructorInjection()
        {
            _container.Bind<Test0>().ToTransient();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test1>(); });

            TestAssert.That(_container.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestUnspecifiedNameFieldInjection()
        {
            _container.Bind<Test0>().ToTransient();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test2>(); });

            TestAssert.That(_container.ValidateResolve<Test2>().Any());
        }

        [Test]
        public void TestTooManySpecified()
        {
            _container.Bind<Test0>().ToTransient();
            _container.Bind<Test0>().ToTransient();

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test1>(); });

            TestAssert.That(_container.ValidateResolve<Test2>().Any());
        }

        [Test]
        public void TestSuccessConstructorInjectionString()
        {
            _container.Bind<Test0>().To(new Test0());
            _container.Bind<Test0>().To(new Test0()).As("foo");

            TestAssert.That(_container.ValidateResolve<Test1>().IsEmpty());
            TestAssert.IsNotNull(_container.Resolve<Test1>());
        }

        [Test]
        public void TestSuccessFieldInjectionString()
        {
            _container.Bind<Test0>().To(new Test0());
            _container.Bind<Test0>().To(new Test0()).As("foo");

            TestAssert.That(_container.ValidateResolve<Test2>().IsEmpty());
            TestAssert.IsNotNull(_container.Resolve<Test2>());
        }

        enum TestEnum
        {
            TestValue1,
            TestValue2,
            TestValue3,
        }

        class Test3
        {
            public Test3(
                [Inject(TestEnum.TestValue2)]
                Test0 test0)
            {
            }
        }

        class Test4
        {

        }

        [Test]
        public void TestFailConstructorInjectionEnum()
        {
            _container.Bind<Test0>().To(new Test0());
            _container.Bind<Test0>().To(new Test0()).As(TestEnum.TestValue1);

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test3>(); });

            TestAssert.That(_container.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestSuccessConstructorInjectionEnum()
        {
            _container.Bind<Test0>().To(new Test0());
            _container.Bind<Test0>().To(new Test0()).As(TestEnum.TestValue2);

            TestAssert.That(_container.ValidateResolve<Test3>().IsEmpty());
            TestAssert.IsNotNull(_container.Resolve<Test3>());
        }

        [Test]
        public void TestFailFieldInjectionEnum()
        {
            _container.Bind<Test0>().To(new Test0());
            _container.Bind<Test0>().To(new Test0()).As(TestEnum.TestValue1);

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test3>(); });

            TestAssert.That(_container.ValidateResolve<Test3>().Any());
        }

        [Test]
        public void TestSuccessFieldInjectionEnum()
        {
            _container.Bind<Test0>().To(new Test0());
            _container.Bind<Test0>().To(new Test0()).As(TestEnum.TestValue3);

            TestAssert.That(_container.ValidateResolve<Test4>().IsEmpty());
            TestAssert.IsNotNull(_container.Resolve<Test4>());
        }
    }
}
