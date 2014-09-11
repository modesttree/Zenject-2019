using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using ModestTree.Zenject.Api.Misc;

namespace ModestTree.Zenject
{
    // Responsibilities:
    // - Run Initialize() on all Iinitializable's, in the order specified by InitPriority
    public class InitializableHandler
    {
        List<InitializableInfo> _initializables = new List<InitializableInfo>();

        public InitializableHandler(
            [InjectOptional]
            List<IInitializable> initializables,
            [InjectOptional]
            List<Tuple<Type, int>> priorities,
            DiContainer container)
        {
            var priorityMap = priorities.ToDictionary(x => x.First, x => x.Second);

            if (Assert.IsEnabled)
            {
                WarnForMissingBindings(initializables, container);
            }

            foreach (var initializable in initializables)
            {
                int priority;
                bool success = priorityMap.TryGetValue(initializable.GetType(), out priority);

                if (!success)
                {
                    //Log.Warn(
                        //String.Format("IInitializable with type '{0}' does not have a priority assigned", //initializable.GetType()));
                }

                _initializables.Add(
                    new InitializableInfo(initializable, success ? (int?)priority : null));
            }
        }

        void WarnForMissingBindings(List<IInitializable> initializables, DiContainer container)
        {
            var boundTypes = initializables.Select(x => x.GetType()).Distinct();
            var unboundTypes = container.AllConcreteTypes.Where(x => x.DerivesFrom<IInitializable>() && !boundTypes.Contains(x));

            foreach (var objType in unboundTypes)
            {
                Log.Warn("Found unbound IInitializable with type '" + objType.Name() + "'");
            }
        }

        int SortCompare(InitializableInfo e1, InitializableInfo e2)
        {
            // Initialize initializables with null priorities last
            if (!e1.Priority.HasValue)
            {
                return 1;
            }

            if (!e2.Priority.HasValue)
            {
                return -1;
            }

            return e1.Priority.Value.CompareTo(e2.Priority.Value);
        }

        public void Initialize()
        {
            _initializables.Sort(SortCompare);

            if (Assert.IsEnabled)
            {
                foreach (var initializable in _initializables.Select(x => x.Initializable).GetDuplicates())
                {
                    Assert.That(false, "Found duplicate IInitializable with type '{0}'".With(initializable.GetType()));
                }
            }

            foreach (var initializable in _initializables)
            {
                //Log.Info("Initializing initializable with type '" + initializable.GetType() + "'");

                try
                {
                    using (ProfileBlock.Start("{0}.Initialize()".With(initializable.Initializable.GetType().Name())))
                    {
                        initializable.Initializable.Initialize();
                    }
                }
                catch (Exception e)
                {
                    throw new ZenjectException(
                        "Error occurred while initializing IInitializable with type '{0}'".With(initializable.Initializable.GetType().Name()), e);
                }
            }
        }

        class InitializableInfo
        {
            public IInitializable Initializable;
            public int? Priority;

            public InitializableInfo(IInitializable initializable, int? priority)
            {
                Initializable = initializable;
                Priority = priority;
            }
        }
    }
}
