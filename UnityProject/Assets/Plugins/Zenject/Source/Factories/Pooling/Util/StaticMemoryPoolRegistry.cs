using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
#if UNITY_EDITOR
    public static class StaticMemoryPoolRegistry
    {
        public static event Action<IMemoryPool> PoolAdded = delegate {};
        public static event Action<IMemoryPool> PoolRemoved = delegate {};

        readonly static List<WeakReference<IMemoryPool>> _pools = new List<WeakReference<IMemoryPool>>();

        public static IEnumerable<WeakReference<IMemoryPool>> Pools
        {
            get { return _pools; }
        }

        public static void Add(IMemoryPool memoryPool)
        {
            _pools.Add(new WeakReference<IMemoryPool>(memoryPool));
            PoolAdded(memoryPool);
        }

        public static void Remove(IMemoryPool memoryPool)
        {
            int index = FindIndex(memoryPool);

            if (index >= 0)
            {
                _pools.RemoveAt(index);
                PoolRemoved(memoryPool);
            }
            else
            {
                Assert.CreateException();
            }
        }

        private static int FindIndex(IMemoryPool memoryPool)
        {
            for (int i = 0; i < _pools.Count; i++)
            {
                IMemoryPool pool;
                if (_pools[i].TryGetTarget(out pool) && pool == memoryPool)
                {
                    return i;
                }
            }
            return -1;
        }
    }
#endif
}
