using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestParameters : ZenjectUnitTestFixture
    {
        class Test1
        {
            public int f1;
            public int f2;

            public Test1(int f1, int f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
        }

        [Test]
        public void TestExtraParametersSameType()
        {
            var test1 = Container.Instantiate<Test1>(new object[] { 5, 10 });

            Assert.That(test1 != null);
            Assert.That(test1.f1 == 5 && test1.f2 == 10);

            var test2 = Container.Instantiate<Test1>(new object[] { 10, 5 });

            Assert.That(test2 != null);
            Assert.That(test2.f1 == 10 && test2.f2 == 5);
        }

        [Test]
        public void TestMissingParameterThrows()
        {
            Container.Bind<Test1>().AsTransient().NonLazy();

            Assert.Throws(
                delegate { Container.Resolve<Test1>(); });
        }
    }
}


