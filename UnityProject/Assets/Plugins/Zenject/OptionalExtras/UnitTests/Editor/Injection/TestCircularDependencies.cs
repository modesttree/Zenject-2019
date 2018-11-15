using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestCircularDependencies : ZenjectUnitTestFixture
    {
        class Test1
        {
            public static int CreateCount;

            [Inject]
            public Test2 Other;

            public Test1()
            {
                CreateCount++;
            }
        }

        class Test2
        {
            public static int CreateCount;

            [Inject]
            public Test1 Other;

            public Test2()
            {
                CreateCount++;
            }
        }

        [Test]
        public void TestFields()
        {
            Test2.CreateCount = 0;
            Test1.CreateCount = 0;

            Container.Bind<Test1>().AsSingle();
            Container.Bind<Test2>().AsSingle();

            var test1 = Container.Resolve<Test1>();
            var test2 = Container.Resolve<Test2>();

            Assert.IsEqual(Test2.CreateCount, 1);
            Assert.IsEqual(Test1.CreateCount, 1);
            Assert.IsEqual(test1.Other, test2);
            Assert.IsEqual(test2.Other, test1);
        }

        class Test3
        {
            public static int CreateCount;

            public Test4 Other;

            public Test3()
            {
                CreateCount++;
            }

            [Inject]
            public void Initialize(Test4 other)
            {
                Other = other;
            }
        }

        class Test4
        {
            public static int CreateCount;

            public Test3 Other;

            public Test4()
            {
                CreateCount++;
            }

            [Inject]
            public void Initialize(Test3 other)
            {
                Other = other;
            }
        }

        [Test]
        public void TestPostInject()
        {
            Test4.CreateCount = 0;
            Test3.CreateCount = 0;

            Container.Bind<Test3>().AsSingle();
            Container.Bind<Test4>().AsSingle();

            var test1 = Container.Resolve<Test3>();
            var test2 = Container.Resolve<Test4>();

            Assert.IsEqual(Test4.CreateCount, 1);
            Assert.IsEqual(Test3.CreateCount, 1);
            Assert.IsEqual(test1.Other, test2);
            Assert.IsEqual(test2.Other, test1);
        }

        class Test5
        {
            public Test5(Test6 Other)
            {
                Assert.IsNotNull(Other);
            }
        }

        class Test6
        {
            public Test6(Test5 other)
            {
                Assert.IsNotNull(other);
            }
        }

        [Test]
        public void TestConstructorInject()
        {
            if (Container.ChecksForCircularDependencies)
            {
                Container.Bind<Test5>().AsSingle().NonLazy();
                Container.Bind<Test6>().AsSingle().NonLazy();

                Assert.Throws(() => Container.Resolve<Test5>());
                Assert.Throws(() => Container.Resolve<Test6>());
            }
        }

        class Test7
        {
            public Test7(Test7 other)
            {
            }
        }

        [Test]
        public void TestSelfDependency()
        {
            if (Container.ChecksForCircularDependencies)
            {
                Container.Bind<Test7>().AsSingle();
                Assert.Throws(() => Container.Instantiate<Test7>());
            }
        }
    }
}


