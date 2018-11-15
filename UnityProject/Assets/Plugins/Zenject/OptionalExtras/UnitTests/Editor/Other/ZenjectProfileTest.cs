using NUnit.Framework;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class ZenjectProfileTest
    {
        class Test0
        {
            public void DoStuff()
            {
            }

            public void DoStuff1()
            {
            }
        }

        class Test1
        {
            [Inject]
            public Test0 TestB
            {
                set;
                get;
            }

            [Inject]
            public Test0 _testC;

            public Test1(Test0 test1, Test0 test2, Test0 test3, Test0 test4)
            {
            }

            public void DoStuff()
            {
            }

            public void DoStuff1()
            {
            }
        }

        class Test2
        {
            [Inject]
            public Test1 TestB
            {
                set;
                get;
            }

            [Inject]
            public Test1 _testC;

            public Test2(Test1 test1, Test1 test2, Test1 test3, Test1 test4)
            {
            }
        }

        [Test]
        // Test speed of resolve function
        public void Test()
        {
            //var container = new DiContainer();
            //container.Bind<Test0>().AsTransient();
            //container.Bind<Test1>().AsTransient();
            //container.Bind<Test2>().AsTransient();

            //var stopwatch = new Stopwatch();

            //stopwatch.Start();

            //for (int i = 0; i < 1000; i++)
            //{
                //var test0 = container.Resolve<Test2>();
                //var test1 = container.Resolve<Test2>();
                //var test2 = container.Resolve<Test2>();
            //}

            //stopwatch.Stop();

            //Log.InfoFormat("time = {0}", stopwatch.Elapsed.TotalSeconds);

            //Assert.That(false);
        }
    }
}


