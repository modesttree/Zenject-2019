using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Conditions
{
    [TestFixture]
    public class TestConditionsTargetInstance : ZenjectUnitTestFixture
    {
        class Test0
        {
        }

        class Test1
        {
            [Inject]
            public Test0 test0;
        }

        Test1 _test1;

        public override void Setup()
        {
            base.Setup();

            _test1 = new Test1();
            Container.Bind<Test0>().AsSingle().When(r => r.ObjectInstance == _test1);
            Container.Bind<Test1>().FromInstance(_test1);
        }

        [Test]
        public void TestTargetConditionError()
        {
            Container.Inject(_test1);

            Assert.That(_test1.test0 != null);
        }
    }
}
