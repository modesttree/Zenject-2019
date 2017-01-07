using System;
using System.Collections.Generic;

namespace Zenject
{
    public class ObjectPool<T> : IPool<T>
        where T : IPoolItem, new()
    {
        private Stack<T> m_unusedItems;
        private Func<T> m_createItemAction;

        public int PoolSize { get; private set; }
        public int TotalActive { get { return PoolSize - TotalInactive; } }
        public int TotalInactive { get { return m_unusedItems.Count; } }

        public ObjectPool() : this(4) { }

        public ObjectPool(int initalialSize)
        {
            m_createItemAction = CreateNew;

            InitialisePool(initalialSize);
        }

        public ObjectPool(int initalialSize, Func<T> createItemAction)
        {
            m_createItemAction = createItemAction;

            InitialisePool(initalialSize);
        }

        public void Dispose()
        {
            // ToDo: Should we mark IPoolItem as IDisposable? Or should we check if T is IDisposable and call dispose?

            m_unusedItems.Clear();
            m_unusedItems = null;
        }

        public T Get()
        {
            T item;

            if (m_unusedItems.Count == 0)
            {
                PoolSize++;

                item = m_createItemAction.Invoke();
            }
            else
            {
                item = m_unusedItems.Pop();
            }

            // Reset the item when it's requested. This is like Initialise.
            // ToDo: Should this be changed to Initialise?
            item.Reset();

            return item;
        }

        public void Return(T item)
        {
            // Check to make sure someone hasn't released twice.
            if (m_unusedItems.Count > 0 && ReferenceEquals(m_unusedItems.Peek(), item))
            {
                throw new InvalidPoolReleaseException();
            }

            m_unusedItems.Push(item);
        }

        private void InitialisePool(int initalialSize)
        {
            m_unusedItems = new Stack<T>(initalialSize);
            PoolSize = initalialSize;

            for (int i = 0; i < initalialSize; i++)
            {
                m_unusedItems.Push(m_createItemAction.Invoke());
            }
        }

        private T CreateNew()
        {
            return new T();
        }
    }

    public class InvalidPoolReleaseException : Exception
    {
        public InvalidPoolReleaseException()
            : base("Can not release item that's already released.")
        {
        }
    }
}