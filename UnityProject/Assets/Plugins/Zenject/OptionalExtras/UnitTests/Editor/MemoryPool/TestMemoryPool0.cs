using System.Collections.Generic;
using NUnit.Framework;
using Assert = ModestTree.Assert;

#pragma warning disable 219

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestMemoryPool0 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestFactoryProperties()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>();

            var pool = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 0);
            Assert.IsEqual(pool.NumInactive, 0);

            var foo = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 1);

            pool.Despawn(foo);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 1);
            Assert.IsEqual(foo.ResetCount, 1);

            foo = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 2);

            var foo2 = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(foo2.ResetCount, 1);

            pool.Despawn(foo);

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 1);
            Assert.IsEqual(foo.ResetCount, 2);

            pool.Despawn(foo2);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 2);
        }

        [Test]
        public void TestFactoryScopeDefault()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>();

            Assert.IsEqual(Container.Resolve<Foo.Pool>(), Container.Resolve<Foo.Pool>());
        }

        [Test]
        public void TestFactoryScopeTransient()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().AsTransient();

            Assert.IsNotEqual(Container.Resolve<Foo.Pool>(), Container.Resolve<Foo.Pool>());
        }

        [Test]
        public void TestFactoryPropertiesDefault()
        {
            Container.BindMemoryPool<Foo>();

            var pool = Container.Resolve<MemoryPool<Foo>>();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 0);
            Assert.IsEqual(pool.NumInactive, 0);

            var foo = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 0);

            pool.Despawn(foo);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 1);

            foo = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 0);

            var foo2 = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 0);

            pool.Despawn(foo);

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 1);

            pool.Despawn(foo2);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 2);
        }

        [Test]
        public void TestExpandDouble()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().ExpandByDoubling();

            var pool = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 0);
            Assert.IsEqual(pool.NumInactive, 0);

            var foo = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 0);

            var foo2 = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 0);

            var foo3 = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 3);
            Assert.IsEqual(pool.NumTotal, 4);
            Assert.IsEqual(pool.NumInactive, 1);

            pool.Despawn(foo2);

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumTotal, 4);
            Assert.IsEqual(pool.NumInactive, 2);

            var foo4 = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 3);
            Assert.IsEqual(pool.NumTotal, 4);
            Assert.IsEqual(pool.NumInactive, 1);
        }

        [Test]
        public void TestFixedSize()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().WithFixedSize(2);

            var pool = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 2);

            var foo = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 1);

            var foo2 = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 0);

            Assert.Throws<PoolExceededFixedSizeException>(() => pool.Spawn());
        }

        [Test]
        public void TestInitialSize()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().WithInitialSize(5);

            var pool = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 5);
            Assert.IsEqual(pool.NumInactive, 5);
        }

        [Test]
        public void TestExpandAndShrinkManually()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>();

            var pool = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 0);
            Assert.IsEqual(pool.NumInactive, 0);

            pool.ExpandBy(2);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 2);

            var foo = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 1);

            pool.ExpandBy(3);

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 5);
            Assert.IsEqual(pool.NumInactive, 4);

            var foo2 = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumTotal, 5);
            Assert.IsEqual(pool.NumInactive, 3);

            var foo3 = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 3);
            Assert.IsEqual(pool.NumTotal, 5);
            Assert.IsEqual(pool.NumInactive, 2);

            pool.ExpandBy(1);

            Assert.IsEqual(pool.NumActive, 3);
            Assert.IsEqual(pool.NumTotal, 6);
            Assert.IsEqual(pool.NumInactive, 3);

            pool.Despawn(foo2);

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumTotal, 6);
            Assert.IsEqual(pool.NumInactive, 4);

            var foo4 = pool.Spawn();

            Assert.IsEqual(pool.NumActive, 3);
            Assert.IsEqual(pool.NumTotal, 6);
            Assert.IsEqual(pool.NumInactive, 3);

            pool.ShrinkBy(1);

            Assert.IsEqual(pool.NumActive, 3);
            Assert.IsEqual(pool.NumTotal, 5);
            Assert.IsEqual(pool.NumInactive, 2);

            pool.Resize(6);

            Assert.IsEqual(pool.NumActive, 3);
            Assert.IsEqual(pool.NumTotal, 9);
            Assert.IsEqual(pool.NumInactive, 6);

            pool.Clear();

            Assert.IsEqual(pool.NumActive, 3);
            Assert.IsEqual(pool.NumTotal, 3);
            Assert.IsEqual(pool.NumInactive, 0);

            Assert.Throws(() => pool.Resize(-1));
            Assert.Throws(() => pool.ShrinkBy(1));
        }

        [Test]
        public void TestMaxSize()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().WithInitialSize(2).WithMaxSize(4);

            var pool = Container.Resolve<Foo.Pool>();

            var foos = new List<Foo>();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 2);

            foos.Add(pool.Spawn());
            foos.Add(pool.Spawn());
            foos.Add(pool.Spawn());
            foos.Add(pool.Spawn());
            foos.Add(pool.Spawn());

            Assert.IsEqual(pool.NumActive, 5);
            Assert.IsEqual(pool.NumTotal, 5);
            Assert.IsEqual(pool.NumInactive, 0);

            pool.Despawn(foos[0]);
            pool.Despawn(foos[1]);
            pool.Despawn(foos[2]);

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumTotal, 5);
            Assert.IsEqual(pool.NumInactive, 3);

            pool.Despawn(foos[3]);
            pool.Despawn(foos[4]);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 4);
            Assert.IsEqual(pool.NumInactive, 4);
        }

        class Bar
        {
            public class Pool : MemoryPool<Bar>
            {
            }
        }

        class Foo
        {
            public int ResetCount
            {
                get; private set;
            }

            public class Pool : MemoryPool<Foo>
            {
                protected override void OnSpawned(Foo foo)
                {
                    foo.ResetCount++;
                }
            }
        }

        [Test]
        public void TestSubContainers()
        {
            Container.BindMemoryPool<Qux, Qux.Pool>()
                .FromSubContainerResolve().ByMethod(InstallQux).NonLazy();

            var pool = Container.Resolve<Qux.Pool>();
            var qux = pool.Spawn();
        }

        void InstallQux(DiContainer subContainer)
        {
            subContainer.Bind<Qux>().AsSingle();
        }

        class Qux
        {
            public class Pool : MemoryPool<Qux>
            {
            }
        }

        [Test]
        [ValidateOnly]
        public void TestIds()
        {
            Container.BindMemoryPool<Foo, Foo.Pool>().WithId("foo").WithInitialSize(5);

            var pool = Container.ResolveId<Foo.Pool>("foo");
        }
    }
}

