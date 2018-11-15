using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestPoolableManager : ZenjectUnitTestFixture
    {
        static int CallCount;

        public class Foo : IPoolable
        {
            public static int SpawnCallCount;
            public static int DespawnCallCount;

            public void OnSpawned()
            {
                SpawnCallCount = CallCount++;
            }

            public void OnDespawned()
            {
                DespawnCallCount = CallCount++;
            }
        }

        public class Bar : IPoolable
        {
            public static int SpawnCallCount;
            public static int DespawnCallCount;

            public void OnSpawned()
            {
                SpawnCallCount = CallCount++;
            }

            public void OnDespawned()
            {
                DespawnCallCount = CallCount++;
            }
        }

        [Test]
        public void TestDefaultOrder()
        {
            Container.Bind<PoolableManager>().AsSingle();
            Container.Bind<IPoolable>().To<Foo>().AsSingle();
            Container.Bind<IPoolable>().To<Bar>().AsSingle();

            var poolManager = Container.Resolve<PoolableManager>();

            CallCount = 1;
            Foo.SpawnCallCount = 0;
            Foo.DespawnCallCount = 0;
            Bar.SpawnCallCount = 0;
            Bar.DespawnCallCount = 0;

            poolManager.TriggerOnSpawned();

            Assert.IsEqual(Foo.SpawnCallCount, 1);
            Assert.IsEqual(Bar.SpawnCallCount, 2);
            Assert.IsEqual(Foo.DespawnCallCount, 0);
            Assert.IsEqual(Bar.DespawnCallCount, 0);

            poolManager.TriggerOnDespawned();

            Assert.IsEqual(Foo.SpawnCallCount, 1);
            Assert.IsEqual(Bar.SpawnCallCount, 2);
            Assert.IsEqual(Foo.DespawnCallCount, 4);
            Assert.IsEqual(Bar.DespawnCallCount, 3);
        }

        [Test]
        public void TestExplicitOrder()
        {
            Container.Bind<PoolableManager>().AsSingle();
            Container.Bind<IPoolable>().To<Foo>().AsSingle();
            Container.Bind<IPoolable>().To<Bar>().AsSingle();

            Container.BindExecutionOrder<Foo>(2);
            Container.BindExecutionOrder<Bar>(1);

            var poolManager = Container.Resolve<PoolableManager>();

            CallCount = 1;
            Foo.SpawnCallCount = 0;
            Foo.DespawnCallCount = 0;
            Bar.SpawnCallCount = 0;
            Bar.DespawnCallCount = 0;

            poolManager.TriggerOnSpawned();

            Assert.IsEqual(Foo.SpawnCallCount, 2);
            Assert.IsEqual(Bar.SpawnCallCount, 1);
            Assert.IsEqual(Foo.DespawnCallCount, 0);
            Assert.IsEqual(Bar.DespawnCallCount, 0);

            poolManager.TriggerOnDespawned();

            Assert.IsEqual(Foo.SpawnCallCount, 2);
            Assert.IsEqual(Bar.SpawnCallCount, 1);
            Assert.IsEqual(Foo.DespawnCallCount, 3);
            Assert.IsEqual(Bar.DespawnCallCount, 4);
        }
    }
}
