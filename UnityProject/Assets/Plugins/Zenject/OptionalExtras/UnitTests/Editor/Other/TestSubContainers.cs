using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestSubContainers : ZenjectUnitTestFixture
    {
        class Test0
        {
        }

        [Test]
        public void TestIsRemoved()
        {
            var subContainer = Container.CreateSubContainer();
            var test1 = new Test0();

            subContainer.Bind<Test0>().FromInstance(test1);

            Assert.That(ReferenceEquals(test1, subContainer.Resolve<Test0>()));

            Assert.Throws(
                delegate { Container.Resolve<Test0>(); });
        }

        class Test1
        {
            [Inject]
            public Test0 Test;
        }

        [Test]
        public void TestCase2()
        {
            Test0 test0;
            Test1 test1;

            var subContainer = Container.CreateSubContainer();
            var test0Local = new Test0();

            subContainer.Bind<Test0>().FromInstance(test0Local);
            subContainer.Bind<Test1>().AsSingle();

            test0 = subContainer.Resolve<Test0>();
            Assert.IsEqual(test0Local, test0);

            test1 = subContainer.Resolve<Test1>();

            Assert.Throws(
                delegate { Container.Resolve<Test0>(); });

            Assert.Throws(
                delegate { Container.Resolve<Test1>(); });

            Container.Bind<Test0>().AsSingle();
            Container.Bind<Test1>().AsSingle();

            Assert.That(Container.Resolve<Test0>() != test0);

            Assert.That(Container.Resolve<Test1>() != test1);
        }

        interface IFoo
        {
        }

        interface IFoo2
        {
        }

        class Foo : IFoo, IFoo2
        {
        }

        [Test]
        public void TestMultipleSingletonDifferentScope()
        {
            IFoo foo1;

            var subContainer1 = Container.CreateSubContainer();
            subContainer1.Bind<IFoo>().To<Foo>().AsSingle();
            foo1 = subContainer1.Resolve<IFoo>();

            var subContainer2 = Container.CreateSubContainer();
            subContainer2.Bind<IFoo>().To<Foo>().AsSingle();
            var foo2 = subContainer2.Resolve<IFoo>();

            Assert.That(foo2 != foo1);
        }
    }
}

