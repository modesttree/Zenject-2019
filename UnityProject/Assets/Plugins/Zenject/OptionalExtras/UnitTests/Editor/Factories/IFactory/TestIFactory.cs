using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestIFactory : ZenjectUnitTestFixture
    {
        [Test]
        public void Test1()
        {
            Container.BindIFactory<Foo>();

            var factory = Container.Resolve<IFactory<Foo>>();

            Assert.IsNotNull(factory.Create());
        }

        [Test]
        public void Test2Error()
        {
            Container.BindIFactory<FooTwo>();

            var factory = Container.Resolve<IFactory<FooTwo>>();

            Assert.Throws(() => factory.Create());
        }

        [Test]
        public void Test2()
        {
            Container.BindIFactory<string, FooTwo>();

            var factory = Container.Resolve<IFactory<string, FooTwo>>();

            Assert.IsEqual(factory.Create("asdf").Value, "asdf");
        }

        [Test]
        public void Test5()
        {
            Container.BindIFactory<string, int, char, long, double, IFooFive>().To<FooFive>();

            var factory = Container.Resolve<IFactory<string, int, char, long, double, IFooFive>>();

            Assert.IsEqual(factory.Create("asdf", 0, 'z', 2, 3.0).P1, "asdf");
        }

        public class Foo
        {
        }

        public class FooTwo
        {
            public FooTwo(string value)
            {
                Value = value;
            }

            public string Value
            {
                get;
                private set;
            }
        }

        public interface IFooFive
        {
            string P1
            {
                get;
            }
        }

        public class FooFive : IFooFive
        {
            string _p1;
            public FooFive(string p1, int p2, char p3, long p4, double p5)
            {
                _p1 = p1;
            }

            public string P1
            {
                get
                {
                    return _p1;
                }
            }
        }
    }
}

