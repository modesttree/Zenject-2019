using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Injection
{
    [TestFixture]
    public class TestStructInjection : ZenjectUnitTestFixture
    {
        struct Test1
        {
        }

        class Test2
        {
            public Test2(Test1 t1)
            {
            }
        }

        [Test]
        public void TestInjectStructIntoClass()
        {
            Container.Bind<Test1>().FromInstance(new Test1()).NonLazy();
            Container.Bind<Test2>().AsSingle().NonLazy();

            Container.ResolveRoots();

            var t2 = Container.Resolve<Test2>();

            Assert.That(t2 != null);
        }

        struct Test3
        {
            [Inject]
            public int ValueField;

            [Inject]
            public string ValueProperty
            {
                get; private set;
            }

            public float ValueConstructor
            {
                get; private set;
            }
        }

        [Test]
        public void TestInjectFieldsOfStruct()
        {
            Container.BindInstance("asdf");
            Container.BindInstance(5);
            Container.Bind<Test3>().AsSingle();

            var test3 = Container.Instantiate<Test3>();

            Assert.IsEqual(test3.ValueProperty, "asdf");
            Assert.IsEqual(test3.ValueField, 5);
        }

        struct Test4
        {
            public Test4(string value)
            {
                Value = value;
            }

            public string Value
            {
                get; private set;
            }
        }

        [Test]
        public void TestInjectConstructorOfStruct()
        {
            Container.BindInstance("asdf");
            Container.Bind<Test4>().AsSingle();

            var test4 = Container.Instantiate<Test4>();

            Assert.IsEqual(test4.Value, "asdf");
        }
    }
}
