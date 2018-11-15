using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestInjectSources
    {
        class Test0
        {
        }

        class Test1
        {
            public Test0 val;

            public Test1(
                [InjectLocal]
                Test0 val)
            {
                this.val = val;
            }
        }

        class Test2
        {
            public Test0 val;

            public Test2(
                [Inject(Source = InjectSources.Parent)]
                Test0 val)
            {
                this.val = val;
            }
        }

        class Test3
        {
            public Test0 val;

            public Test3(
                [Inject(Source = InjectSources.AnyParent)]
                Test0 val)
            {
                this.val = val;
            }
        }

        class Test4
        {
            public Test0 val;

            public Test4(
                [Inject(Source = InjectSources.Any)]
                Test0 val)
            {
                this.val = val;
            }
        }

        [Test]
        public void TestAny()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();

            rootContainer.Bind<Test0>().AsSingle();
            sub1.Bind<Test4>().AsSingle();

            Assert.IsNotNull(sub1.Resolve<Test4>());
        }

        [Test]
        public void TestLocal1()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();

            rootContainer.Bind<Test0>().AsSingle();
            sub1.Bind<Test1>().AsSingle();

            Assert.Throws(() => sub1.Resolve<Test1>());
        }

        [Test]
        public void TestLocal2()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();

            sub1.Bind<Test0>().AsSingle();
            sub1.Bind<Test1>().AsSingle();

            Assert.IsNotNull(sub1.Resolve<Test1>());
        }

        [Test]
        public void TestParent1()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();

            rootContainer.Bind<Test0>().AsSingle();
            sub1.Bind<Test2>().AsSingle();

            Assert.IsNotNull(sub1.Resolve<Test2>());
        }

        [Test]
        public void TestParent2()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();
            var sub2 = sub1.CreateSubContainer();

            rootContainer.Bind<Test0>().AsSingle();
            sub2.Bind<Test2>().AsSingle();

            Assert.Throws(() => sub2.Resolve<Test2>());
        }

        [Test]
        public void TestParent3()
        {
            var rootContainer = new DiContainer();

            rootContainer.Bind<Test0>().AsSingle();
            rootContainer.Bind<Test2>().AsSingle();

            Assert.Throws(() => rootContainer.Resolve<Test2>());
        }

        [Test]
        public void TestParentAny1()
        {
            var rootContainer = new DiContainer();
            var sub1 = rootContainer.CreateSubContainer();
            var sub2 = sub1.CreateSubContainer();

            rootContainer.Bind<Test0>().AsSingle();
            sub2.Bind<Test3>().AsSingle();

            Assert.IsNotNull(sub2.Resolve<Test3>());
        }
    }
}



