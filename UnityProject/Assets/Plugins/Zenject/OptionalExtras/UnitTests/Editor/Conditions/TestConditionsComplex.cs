using System.Linq;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Conditions
{
    [TestFixture]
    public class TestConditionsComplex : ZenjectUnitTestFixture
    {
        class Foo
        {
        }

        class Bar
        {
            public Foo Foo;

            public Bar(Foo foo)
            {
                Foo = foo;
            }
        }

        [Test]
        public void TestCorrespondingIdentifiers()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();

            Container.Bind<Bar>().WithId("Bar1").AsTransient().NonLazy();
            Container.Bind<Bar>().WithId("Bar2").AsTransient().NonLazy();

            Container.BindInstance(foo1).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && Equals(x.Identifier, "Bar1")).Any());
            Container.BindInstance(foo2).When(c => c.ParentContexts.Where(x => x.MemberType == typeof(Bar) && Equals(x.Identifier, "Bar2")).Any());

            Assert.IsEqual(Container.ResolveId<Bar>("Bar1").Foo, foo1);
            Assert.IsEqual(Container.ResolveId<Bar>("Bar2").Foo, foo2);
        }
    }
}
