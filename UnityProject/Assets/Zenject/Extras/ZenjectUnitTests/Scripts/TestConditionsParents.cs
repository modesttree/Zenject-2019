using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Exceptions;
using NUnit.Framework;
using System.Linq;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestConditionsParents : TestWithContainer
    {
        class Test0
        {
        }

        interface ITest1
        {
        }

        class Test1 : ITest1
        {
            public Test0 test0;

            public Test1(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test2 : ITest1
        {
            public Test0 test0;

            public Test2(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test3 : ITest1
        {
            public Test1 test1;

            public Test3(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        class Test4 : ITest1
        {
            public Test1 test1;

            public Test4(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        [Test]
        public void TestCase1()
        {
            _container.Bind<Test1>().ToSingle();
            _container.Bind<Test0>().ToSingle().When(c => c.ParentTypes.Contains(typeof(Test2)));

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<Test1>(); });

            TestAssert.That(_container.ValidateResolve<Test0>().Any());
        }

        [Test]
        public void TestCase2()
        {
            _container.Bind<Test1>().ToSingle();
            _container.Bind<Test0>().ToSingle().When(c => c.ParentTypes.Contains(typeof(Test1)));

            TestAssert.That(_container.ValidateResolve<Test1>().IsEmpty());
            var test1 = _container.Resolve<Test1>();
            TestAssert.That(test1 != null);
        }

        // Test using parents to look deeper up the heirarchy..
        [Test]
        public void TestCase3()
        {
            var t0a = new Test0();
            var t0b = new Test0();

            _container.Bind<Test3>().ToSingle();
            _container.Bind<Test4>().ToSingle();
            _container.Bind<Test1>().ToTransient();

            _container.Bind<Test0>().To(t0a).When(c => c.ParentTypes.Contains(typeof(Test3)));
            _container.Bind<Test0>().To(t0b).When(c => c.ParentTypes.Contains(typeof(Test4)));

            TestAssert.That(_container.ValidateResolve<Test3>().IsEmpty());
            var test3 = _container.Resolve<Test3>();

            TestAssert.That(_container.ValidateResolve<Test4>().IsEmpty());
            var test4 = _container.Resolve<Test4>();

            TestAssert.That(ReferenceEquals(test3.test1.test0, t0a));
            TestAssert.That(ReferenceEquals(test4.test1.test0, t0b));
        }

        [Test]
        public void TestCase4()
        {
            _container.Bind<ITest1>().ToSingle<Test2>();
            _container.Bind<Test0>().ToSingle().When(c => c.ParentTypes.Contains(typeof(ITest1)));

            TestAssert.Throws<ZenjectResolveException>(
                delegate { _container.Resolve<ITest1>(); });

            TestAssert.That(_container.ValidateResolve<Test1>().Any());
        }

        [Test]
        public void TestCase5()
        {
            _container.Bind<ITest1>().ToSingle<Test2>();
            _container.Bind<Test0>().ToSingle().When(c => c.ParentTypes.Contains(typeof(Test2)));

            TestAssert.That(_container.ValidateResolve<ITest1>().IsEmpty());
            var test1 = _container.Resolve<ITest1>();
            TestAssert.That(test1 != null);
        }

        [Test]
        public void TestCase6()
        {
            _container.Bind<ITest1>().ToSingle<Test2>();
            _container.Bind<Test0>().ToSingle().When(c => c.ParentTypes.Where(x => typeof(ITest1).IsAssignableFrom(x)).Any());

            TestAssert.That(_container.ValidateResolve<ITest1>().IsEmpty());
            var test1 = _container.Resolve<ITest1>();
            TestAssert.That(test1 != null);
        }
    }
}

