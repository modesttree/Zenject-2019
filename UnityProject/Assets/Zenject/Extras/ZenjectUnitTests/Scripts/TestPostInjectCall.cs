using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using ModestTree.Zenject.Api.Misc;
using NUnit.Framework;
using TestAssert=NUnit.Framework.Assert;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestPostInjectCall : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
        }

        class Test2
        {
        }

        class Test3
        {
            public bool HasInitialized;
            public bool HasInitialized2;

            [Inject]
            public Test1 test1 = null;

            [Inject]
            public Test0 test0 = null;

            Test2 _test2;

            public Test3(Test2 test2)
            {
                _test2 = test2;
            }

            [PostInject]
            public void Init()
            {
                TestAssert.That(!HasInitialized);
                TestAssert.IsNotNull(test1);
                TestAssert.IsNotNull(test0);
                TestAssert.IsNotNull(_test2);
                HasInitialized = true;
            }

            [PostInject]
            void TestPrivatePostInject()
            {
                HasInitialized2 = true;
            }
        }

        [Test]
        public void Test()
        {
            _container.Bind<Test0>().ToSingle();
            _container.Bind<Test1>().ToSingle();
            _container.Bind<Test2>().ToSingle();
            _container.Bind<Test3>().ToSingle();

            TestAssert.That(_container.ValidateResolve<Test3>().IsEmpty());
            var test3 = _container.Resolve<Test3>();
            TestAssert.That(test3.HasInitialized);
            TestAssert.That(test3.HasInitialized2);
        }

        public class SimpleBase
        {
            public bool WasCalled = false;

            [PostInject]
            void Init()
            {
                WasCalled = true;
            }
        }

        public class SimpleDerived : SimpleBase
        {
        }

        [Test]
        public void TestPrivateBaseClassPostInject()
        {
            _container.Bind<SimpleBase>().ToSingle<SimpleDerived>();

            var simple = _container.Resolve<SimpleBase>();

            TestAssert.That(simple.WasCalled);
        }

        [Test]
        public void TestInheritance()
        {
            _container.Bind<IFoo>().ToSingle<FooDerived>();

            TestAssert.That(_container.ValidateResolve<IFoo>().IsEmpty());
            var foo = _container.Resolve<IFoo>();

            TestAssert.That(((FooDerived)foo).WasDerivedCalled);
            TestAssert.That(((FooBase)foo).WasBaseCalled);
            TestAssert.That(((FooDerived)foo).WasDerivedCalled2);
            TestAssert.That(((FooBase)foo).WasBaseCalled2);
        }

        [Test]
        public void TestInheritanceOrder()
        {
            _container.Bind<IFoo>().ToSingle<FooDerived2>();

            // base post inject methods should be called first
            _initOrder = 0;
            FooBase.BaseCallOrder = 0;
            FooDerived.DerivedCallOrder = 0;
            FooDerived2.Derived2CallOrder = 0;

            _container.Resolve<IFoo>();

            //Log.Info("FooBase.BaseCallOrder = {0}".With(FooBase.BaseCallOrder));
            //Log.Info("FooDerived.DerivedCallOrder = {0}".With(FooDerived.DerivedCallOrder));

            TestAssert.AreEqual(FooBase.BaseCallOrder, 0);
            TestAssert.AreEqual(FooDerived.DerivedCallOrder, 1);
            TestAssert.AreEqual(FooDerived2.Derived2CallOrder, 2);
        }

        static int _initOrder;

        interface IFoo
        {
        }

        class FooBase : IFoo
        {
            public bool WasBaseCalled;
            public bool WasBaseCalled2;
            public static int BaseCallOrder;

            [PostInject]
            void TestBase()
            {
                TestAssert.That(!WasBaseCalled);
                WasBaseCalled = true;
                BaseCallOrder = _initOrder++;
            }

            [PostInject]
            public virtual void TestVirtual1()
            {
                TestAssert.That(!WasBaseCalled2);
                WasBaseCalled2 = true;
            }
        }

        class FooDerived : FooBase
        {
            public bool WasDerivedCalled;
            public bool WasDerivedCalled2;
            public static int DerivedCallOrder;

            [PostInject]
            void TestDerived()
            {
                TestAssert.That(!WasDerivedCalled);
                WasDerivedCalled = true;
                DerivedCallOrder = _initOrder++;
            }

            public override void TestVirtual1()
            {
                base.TestVirtual1();
                TestAssert.That(!WasDerivedCalled2);
                WasDerivedCalled2 = true;
            }
        }

        class FooDerived2 : FooDerived
        {
            public bool WasDerived2Called;
            public static int Derived2CallOrder;

            [PostInject]
            public void TestVirtual2()
            {
                TestAssert.That(!WasDerived2Called);
                WasDerived2Called = true;
                Derived2CallOrder = _initOrder++;
            }
        }
    }
}

