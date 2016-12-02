using System.Collections.Generic;
using NUnit.Framework;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestPoolFactoryTo0 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSelf()
        {
            Assert.IsNotNull(new Foo.Factory().Get());
        }

        [Test]
        public void TestPool()
        {
            Foo.Factory factory = new Foo.Factory();

            List<Foo> items = new List<Foo>();

            for (int count = 4; count > 0; count--)
            {
                items.Add(factory.Get());

                Assert.That(factory.TotalActive == items.Count);
                Assert.That(factory.TotalInactive == count - 1);
                Assert.That(factory.PoolSize == 4);
            }

            items.Add(factory.Get());

            Assert.That(factory.TotalActive == 5);
            Assert.That(factory.TotalInactive == 0);
            Assert.That(factory.PoolSize == 5);


            for (int i = 0; i < items.Count; i++)
            {
                factory.Return(items[i]);

                Assert.That(factory.TotalActive == items.Count - (i + 1));
                Assert.That(factory.TotalInactive == i + 1);
                Assert.That(factory.PoolSize == 5);
            }
        }
        
        private class Foo : IPoolItem
        {
            public Foo()
            {

            }

            public void Reset()
            {
                
            }

            public class Factory : PoolFactory<Foo>
            {
            }
        }
    }
}

