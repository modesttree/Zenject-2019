using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    /// <summary>
    /// A modified version of PoolableManager that adds a generic argument, allowing
    /// the passing of a parameter to all IPoolable<T> objects in the container.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomPoolableManager<T>
    {
        readonly List<IPoolable<T>> _poolables;

        bool _isSpawned;

        public CustomPoolableManager(
            [InjectLocal]
            List<IPoolable<T>> poolables,
            [Inject(Optional = true, Source = InjectSources.Local)]
            List<ValuePair<Type, int>> priorities)
        {
            _poolables = poolables.Select(x => CreatePoolableInfo(x, priorities))
                .OrderBy(x => x.Priority).Select(x => x.Poolable).ToList();
        }

        CustomPoolableInfo CreatePoolableInfo(IPoolable<T> poolable, List<ValuePair<Type, int>> priorities)
        {
            var match = priorities.Where(x => poolable.GetType().DerivesFromOrEqual(x.First)).Select(x => (int?)(x.Second)).SingleOrDefault();
            int priority = match.HasValue ? match.Value : 0;

            return new CustomPoolableInfo(poolable, priority);
        }

        public void TriggerOnSpawned(T param)
        {
            Assert.That(!_isSpawned);
            _isSpawned = true;

            for (int i = 0; i < _poolables.Count; i++)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.OnSpawned", _poolables[i].GetType()))
#endif
                {
                    _poolables[i].OnSpawned(param);
                }
            }
        }

        public void TriggerOnDespawned()
        {
            Assert.That(_isSpawned);
            _isSpawned = false;

            // Call OnDespawned in the reverse order just like how dispose works
            for (int i = _poolables.Count - 1; i >= 0; i--)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.OnDespawned", _poolables[i].GetType()))
#endif
                {
                    _poolables[i].OnDespawned();
                }
            }
        }

        struct CustomPoolableInfo
        {
            public IPoolable<T> Poolable;
            public int Priority;

            public CustomPoolableInfo(IPoolable<T> poolable, int priority)
            {
                Poolable = poolable;
                Priority = priority;
            }
        }
    }
}
