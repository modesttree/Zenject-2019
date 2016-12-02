using System;

namespace Zenject
{
    public interface IPool<T> : IDisposable
        where T : IPoolItem
    {
        int PoolSize { get; }
        int TotalActive { get; }
        int TotalInactive { get; }

        T Get();
        void Return(T item);
    }
}