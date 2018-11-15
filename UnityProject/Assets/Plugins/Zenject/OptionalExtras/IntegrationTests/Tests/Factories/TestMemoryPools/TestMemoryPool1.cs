#if UNITY_EDITOR

using System.Collections;
using ModestTree;
using UnityEngine.TestTools;

#pragma warning disable 219

namespace Zenject.Tests.Bindings
{
    public class TestMemoryPool1 : ZenjectIntegrationTestFixture
    {
        [UnityTest]
        public IEnumerator TestFactoryProperties()
        {
            PreInstall();
            Container.BindMemoryPool<Foo, Foo.Pool>();

            PostInstall();

            var pool = Container.Resolve<Foo.Pool>();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 0);
            Assert.IsEqual(pool.NumInactive, 0);

            var foo = pool.Spawn("asdf");

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 1);
            Assert.IsEqual(foo.Value, "asdf");

            pool.Despawn(foo);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 1);
            Assert.IsEqual(foo.ResetCount, 1);

            foo = pool.Spawn("zxcv");

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 1);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(foo.ResetCount, 2);
            Assert.IsEqual(foo.Value, "zxcv");

            var foo2 = pool.Spawn("qwer");

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(foo2.ResetCount, 1);
            Assert.IsEqual(foo2.Value, "qwer");

            pool.Despawn(foo);

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 1);
            Assert.IsEqual(foo.ResetCount, 2);

            pool.Despawn(foo2);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumTotal, 2);
            Assert.IsEqual(pool.NumInactive, 2);

            pool.Spawn("zxcv");
            pool.Spawn("bxzc");
            pool.Spawn("bxzc");

            Assert.IsEqual(pool.NumActive, 3);
            Assert.IsEqual(pool.NumTotal, 3);
            Assert.IsEqual(pool.NumInactive, 0);
            yield break;
        }

        class Foo
        {
            public string Value
            {
                get;
                private set;
            }

            public int ResetCount
            {
                get; private set;
            }

            public class Pool : MemoryPool<string, Foo>
            {
                protected override void Reinitialize(string value, Foo foo)
                {
                    foo.Value = value;
                    foo.ResetCount++;
                }
            }
        }

        [UnityTest]
        public IEnumerator TestAbstractMemoryPoolValidate()
        {
            TestAbstractMemoryPoolInternal();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestAbstractMemoryPool()
        {
            TestAbstractMemoryPoolInternal();

            var pool = Container.Resolve<BarPool>();

            var foo = pool.Spawn(5);

            Assert.IsEqual(foo.GetType(), typeof(Bar));
            yield break;
        }

        void TestAbstractMemoryPoolInternal()
        {
            PreInstall();
            Container.BindMemoryPool<IBar, BarPool>()
                .WithInitialSize(3).To<Bar>().NonLazy();

            PostInstall();
        }

        public interface IBar
        {
        }

        public class Bar : IBar
        {
        }

        public class BarPool : MemoryPool<int, IBar>
        {
        }
    }
}

#endif
