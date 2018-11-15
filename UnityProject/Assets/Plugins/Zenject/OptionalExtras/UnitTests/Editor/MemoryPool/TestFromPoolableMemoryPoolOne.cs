using System;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromPoolableMemoryPoolOne : ZenjectUnitTestFixture
    {
        public class Foo : IPoolable<string, IMemoryPool>, IDisposable
        {
            IMemoryPool _pool;
            string _data;
            string _initialData;

            public Foo(string initialData)
            {
                _initialData = initialData;
                SetDefaults();
            }

            public string InitialData
            {
                get { return _initialData; }
            }

            public IMemoryPool Pool
            {
                get { return _pool; }
            }

            public string Data
            {
                get { return _data; }
            }

            void SetDefaults()
            {
                _pool = null;
                _data = null;
            }

            public void Dispose()
            {
                _pool.Despawn(this);
            }

            public void OnDespawned()
            {
                _data = null;
                _pool = null;
                SetDefaults();
            }

            public void OnSpawned(string data, IMemoryPool pool)
            {
                _pool = pool;
                _data = data;
            }

            public class Factory : PlaceholderFactory<string, Foo>
            {
            }
        }

        [Test]
        public void Test1()
        {
            Container.BindFactory<string, Foo, Foo.Factory>().FromPoolableMemoryPool(x => x.WithInitialSize(2).WithArguments("blurg"));

            var factory = Container.Resolve<Foo.Factory>();

            var foo = factory.Create("asdf");

            Assert.IsEqual(foo.InitialData, "blurg");

            var pool = foo.Pool;

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 1);

            Assert.IsEqual(foo.Data, "asdf");

            foo.Dispose();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 2);
            Assert.IsEqual(foo.Data, null);
        }
    }
}
