using System.Linq;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Conditions
{
    [TestFixture]
    public class TestConditionsParents : ZenjectUnitTestFixture
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
            Container.Bind<Test1>().AsSingle().NonLazy();
            Container.Bind<Test0>().AsSingle().When(c => c.AllObjectTypes.Contains(typeof(Test2)));

            Assert.Throws(
                delegate { Container.Resolve<Test1>(); });
        }

        [Test]
        public void TestCase2()
        {
            Container.Bind<Test1>().AsSingle().NonLazy();
            Container.Bind<Test0>().AsSingle().When(c => c.AllObjectTypes.Contains(typeof(Test1)));

            var test1 = Container.Resolve<Test1>();
            Assert.That(test1 != null);
        }

        // Test using parents to look deeper up the heirarchy..
        [Test]
        public void TestCase3()
        {
            var t0a = new Test0();
            var t0b = new Test0();

            Container.Bind<Test3>().AsSingle();
            Container.Bind<Test4>().AsSingle();
            Container.Bind<Test1>().AsTransient();

            Container.Bind<Test0>().FromInstance(t0a).When(c => c.AllObjectTypes.Contains(typeof(Test3)));
            Container.Bind<Test0>().FromInstance(t0b).When(c => c.AllObjectTypes.Contains(typeof(Test4)));

            var test3 = Container.Resolve<Test3>();

            var test4 = Container.Resolve<Test4>();

            Assert.That(ReferenceEquals(test3.test1.test0, t0a));
            Assert.That(ReferenceEquals(test4.test1.test0, t0b));
        }

        [Test]
        public void TestCase4()
        {
            Container.Bind<ITest1>().To<Test2>().AsSingle().NonLazy();
            Container.Bind<Test0>().AsSingle().When(c => c.AllObjectTypes.Contains(typeof(ITest1)));

            Assert.Throws(
                delegate { Container.Resolve<ITest1>(); });
        }

        [Test]
        public void TestCase5()
        {
            Container.Bind<ITest1>().To<Test2>().AsSingle().NonLazy();
            Container.Bind<Test0>().AsSingle().When(c => c.AllObjectTypes.Contains(typeof(Test2)));

            var test1 = Container.Resolve<ITest1>();
            Assert.That(test1 != null);
        }

        [Test]
        public void TestCase6()
        {
            Container.Bind<ITest1>().To<Test2>().AsSingle().NonLazy();
            Container.Bind<Test0>().AsSingle().When(c => c.AllObjectTypes.Where(x => typeof(ITest1).IsAssignableFrom(x)).Any());

            var test1 = Container.Resolve<ITest1>();
            Assert.That(test1 != null);
        }
    }
}

