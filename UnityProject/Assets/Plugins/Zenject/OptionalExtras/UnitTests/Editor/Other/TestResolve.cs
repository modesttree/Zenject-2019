using System.Collections.Generic;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestResolve : ZenjectUnitTestFixture
    {
        class Foo
        {
        }

        [Test]
        public void TestResolveAmbiguousBindings1()
        {
            var f1 = new Foo();
            var f2 = new Foo();

            // Should always choose the binding with condition when forced to choose
            Container.BindInstance(f1);
            Container.BindInstance(f2).When(_ => true);

            Assert.IsEqual(Container.Resolve<Foo>(), f2);
        }


        [Test]
        public void TestResolveAmbiguousBindings2()
        {
            var f1 = new Foo();
            var f2 = new Foo();

            // Order shouldn't matter
            Container.BindInstance(f2).When(_ => true);
            Container.BindInstance(f1);

            Assert.IsEqual(Container.Resolve<Foo>(), f2);
        }

        [Test]
        public void TestDirectListBindings1()
        {
            var f1 = new Foo();

            Container.BindInstance(f1);

            Assert.IsEqual(Container.Instantiate<Bar>().Foos[0], f1);

            var l1 = new List<Foo>();

            Container.BindInstance(l1);

            // Direct list bindings should override the automatic list bindings
            Assert.IsEqual(Container.Instantiate<Bar>().Foos, l1);
        }

        class Bar
        {
            public List<Foo> Foos;

            public Bar(List<Foo> foos)
            {
                Foos = foos;
            }
        }
    }
}



