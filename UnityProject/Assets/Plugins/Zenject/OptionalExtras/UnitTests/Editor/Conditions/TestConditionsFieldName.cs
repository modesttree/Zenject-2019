using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Conditions
{
    [TestFixture]
    public class TestConditionsFieldName : ZenjectUnitTestFixture
    {
        class Test0
        {

        }

        class Test1
        {
            public Test1(Test0 name1)
            {
            }
        }

        class Test2
        {
            public Test2(Test0 name2)
            {
            }
        }

        public override void Setup()
        {
            base.Setup();
            Container.Bind<Test0>().AsSingle().When(r => r.MemberName == "name1");
        }

        [Test]
        public void TestNameConditionError()
        {
            Container.Bind<Test2>().AsSingle().NonLazy();

            Assert.Throws(
                delegate { Container.Resolve<Test2>(); });
        }

        [Test]
        public void TestNameConditionSuccess()
        {
            Container.Bind<Test1>().AsSingle().NonLazy();

            var test1 = Container.Resolve<Test1>();

            Assert.That(test1 != null);
        }
    }
}


