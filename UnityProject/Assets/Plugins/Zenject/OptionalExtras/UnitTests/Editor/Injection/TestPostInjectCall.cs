using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestPostInjectCall : ZenjectUnitTestFixture
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
            public Test1 test1;

            [Inject]
            public Test0 test0;

            Test2 _test2;

            public Test3(Test2 test2)
            {
                _test2 = test2;
            }

            [Inject]
            public void Init()
            {
                Assert.That(!HasInitialized);
                Assert.IsNotNull(test1);
                Assert.IsNotNull(test0);
                Assert.IsNotNull(_test2);
                HasInitialized = true;
            }

            [Inject]
            void TestPrivatePostInject()
            {
                HasInitialized2 = true;
            }
        }

        [Test]
        public void Test()
        {
            Container.Bind<Test0>().AsSingle().NonLazy();
            Container.Bind<Test1>().AsSingle().NonLazy();
            Container.Bind<Test2>().AsSingle().NonLazy();
            Container.Bind<Test3>().AsSingle().NonLazy();

            var test3 = Container.Resolve<Test3>();
            Assert.That(test3.HasInitialized);
            Assert.That(test3.HasInitialized2);
        }

        public class SimpleBase
        {
            public bool WasCalled;

            [Inject]
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
            Container.Bind<SimpleBase>().To<SimpleDerived>().AsSingle().NonLazy();

            var simple = Container.Resolve<SimpleBase>();

            Assert.That(simple.WasCalled);
        }

        [Test]
        public void TestInheritance()
        {
            Container.Bind<IFoo>().To<FooDerived>().AsSingle().NonLazy();

            var foo = Container.Resolve<IFoo>();

            Assert.That(((FooDerived)foo).WasDerivedCalled);
            Assert.That(((FooBase)foo).WasBaseCalled);
            Assert.That(((FooDerived)foo).WasDerivedCalled2);
            Assert.That(((FooBase)foo).WasBaseCalled2);
        }

        [Test]
        public void TestInheritanceOrder()
        {
            Container.Bind<IFoo>().To<FooDerived2>().AsSingle().NonLazy();

            // base post inject methods should be called first
            _initOrder = 0;
            FooBase.BaseCallOrder = 0;
            FooDerived.DerivedCallOrder = 0;
            FooDerived2.Derived2CallOrder = 0;

            Container.Resolve<IFoo>();

            //Log.Info("FooBase.BaseCallOrder = {0}".Fmt(FooBase.BaseCallOrder));
            //Log.Info("FooDerived.DerivedCallOrder = {0}".Fmt(FooDerived.DerivedCallOrder));

            Assert.IsEqual(FooBase.BaseCallOrder, 0);
            Assert.IsEqual(FooDerived.DerivedCallOrder, 1);
            Assert.IsEqual(FooDerived2.Derived2CallOrder, 2);
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

            [Inject]
            void TestBase()
            {
                Assert.That(!WasBaseCalled);
                WasBaseCalled = true;
                BaseCallOrder = _initOrder++;
            }

            [Inject]
            public virtual void TestVirtual1()
            {
                Assert.That(!WasBaseCalled2);
                WasBaseCalled2 = true;
            }
        }

        class FooDerived : FooBase
        {
            public bool WasDerivedCalled;
            public bool WasDerivedCalled2;
            public static int DerivedCallOrder;

            [Inject]
            void TestDerived()
            {
                Assert.That(!WasDerivedCalled);
                WasDerivedCalled = true;
                DerivedCallOrder = _initOrder++;
            }

            public override void TestVirtual1()
            {
                base.TestVirtual1();
                Assert.That(!WasDerivedCalled2);
                WasDerivedCalled2 = true;
            }
        }

        class FooDerived2 : FooDerived
        {
            public bool WasDerived2Called;
            public static int Derived2CallOrder;

            [Inject]
            public void TestVirtual2()
            {
                Assert.That(!WasDerived2Called);
                WasDerived2Called = true;
                Derived2CallOrder = _initOrder++;
            }
        }
    }
}

