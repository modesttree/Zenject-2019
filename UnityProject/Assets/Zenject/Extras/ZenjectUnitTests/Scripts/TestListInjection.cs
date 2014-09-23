using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Exceptions;
using ModestTree.Zenject.Api.Misc;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestListInjection : TestWithContainer
    {
        class Test1
        {
            public Test1(List<int> values)
            {
            }
        }

        class Test2
        {
            public Test2( 
                [InjectOptional] List<int> values)
            {
            }
        }

        class Test3
        {
            [Inject]
            public List<int> values = null;
        }

        class Test4
        {
            [InjectOptional]
            public List<int> values = null;
        }

        [Test]
        [ExpectedException]
        public void TestCase1()
        {
            _container.Bind<Test1>().ToSingle();

            _container.ResolveMany<Test1>();
        }

        [Test]
        public void TestCase2()
        {
            _container.Bind<Test2>().ToSingle();

            TestAssert.That(_container.ValidateResolve<Test2>().IsEmpty());
            var result = _container.ResolveMany<Test2>();

            TestAssert.That(result != null);
        }

        [Test]
        [ExpectedException(typeof(ZenjectResolveException))]
        public void TestCase3()
        {
            _container.Bind<Test3>().ToSingle();

            _container.ResolveMany<Test3>();
        }

        [Test]
        public void TestCase4()
        {
            _container.Bind<Test4>().ToSingle();

            var result = _container.ResolveMany<Test4>();

            TestAssert.That(result != null);
        }
    }
}



