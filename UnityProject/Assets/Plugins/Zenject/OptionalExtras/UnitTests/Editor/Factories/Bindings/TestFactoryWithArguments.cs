using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFactoryWithArguments : ZenjectUnitTestFixture
    {
        [Test]
        public void TestWithArguments1()
        {
            Container.BindFactory<Foo, Foo.Factory>().WithArguments("asdf");

            Assert.IsEqual(Container.Resolve<Foo.Factory>().Create().Value, "asdf");
        }

        [Test]
        public void TestWithFactoryArguments1()
        {
            Container.BindFactory<Bar, Bar.Factory>().WithFactoryArguments("asdf");

            Assert.IsEqual(Container.Resolve<Bar.Factory>().Create().Value, "asdf");
        }

        public class Foo
        {
            public Foo(string value)
            {
                Value = value;
            }

            public string Value
            {
                get; private set;
            }

            public class Factory : PlaceholderFactory<Foo>
            {
            }
        }

        public class Bar
        {
            public string Value
            {
                get; private set;
            }

            public class Factory : PlaceholderFactory<Bar>
            {
                string _value;

                public Factory(string value)
                {
                    _value = value;
                }

                public override Bar Create()
                {
                    var bar = base.Create();
                    bar.Value = _value;
                    return bar;
                }
            }
        }
    }
}

