using System;
using System.Collections.Generic;

namespace Zenject
{
    public abstract class DynamicPoolFactory<TValue> : DynamicFactory<TValue>
        where TValue : IPoolItem, new()
    {
        public int PoolSize { get { return Pool.PoolSize; } }
        public int TotalActive { get { return Pool.TotalActive; } }
        public int TotalInactive { get { return Pool.TotalInactive; } }

        protected ObjectPool<TValue> Pool { get; set; }
    }

    public class PoolFactory<TValue> : DynamicPoolFactory<TValue>, IPoolFactory<TValue>
        where TValue : IPoolItem, new()
    {
        // If you were hoping to override this method, use BindFactory<>.ToFactory instead
        public TValue Get()
        {
            if (Pool == null)
            {
                Pool = new ObjectPool<TValue>(4, CreateInternal);
            }

            return Pool.Get();
        }

        public void Return(TValue item)
        {
            Pool.Return(item);
        }

        private TValue CreateInternal()
        {
            return CreateInternal(new List<TypeValuePair>());
        }

        protected sealed override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield break;
            }
        }
    }
}