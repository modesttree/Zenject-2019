using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests
{
    [TestFixture]
    public class TestArrayPool : ZenjectUnitTestFixture
    {
        [Test]
        public void RunTest()
        {
            var pool = ArrayPool<string>.GetPool(2);

            pool.Clear();
            pool.ClearActiveCount();

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(pool.NumTotal, 0);

            var arr1 = pool.Spawn();

            Assert.IsEqual(arr1.Length, 2);

            arr1[0] = "asdf";
            arr1[1] = "zbx";

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(pool.NumTotal, 1);

            pool.Despawn(arr1);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumInactive, 1);
            Assert.IsEqual(pool.NumTotal, 1);

            var arr2 = pool.Spawn();

            Assert.IsEqual(arr2.Length, 2);
            Assert.IsNull(arr2[0]);
            Assert.IsNull(arr2[1]);

            Assert.IsEqual(arr2.Length, 2);
            Assert.IsEqual(arr2, arr1);

            Assert.IsEqual(pool.NumActive, 1);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(pool.NumTotal, 1);

            var arr3 = pool.Spawn();

            Assert.IsNotEqual(arr2, arr3);

            Assert.IsEqual(pool.NumActive, 2);
            Assert.IsEqual(pool.NumInactive, 0);
            Assert.IsEqual(pool.NumTotal, 2);

            pool.Despawn(arr3);
            pool.Despawn(arr2);

            Assert.IsEqual(pool.NumActive, 0);
            Assert.IsEqual(pool.NumInactive, 2);
            Assert.IsEqual(pool.NumTotal, 2);

            Assert.Throws(() => pool.Despawn(arr3));
        }
    }
}

