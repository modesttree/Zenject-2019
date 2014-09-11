using System;
using System.Collections.Generic;
using ModestTree.Zenject.Api;
using System.Linq;
using ModestTree.Zenject.Api.Misc;

namespace ModestTree.Zenject
{
    // This class can be bound to IKernel in cases where you want complete
    // control over when all tasks are updated
    // Useful in cases where you are running a sub-container
    public class StandardKernel : IKernel
    {
        LinkedList<TickableInfo> _sortedTasks = new LinkedList<TickableInfo>();
        LinkedList<TickableInfo> _unsortedTasks = new LinkedList<TickableInfo>();

        List<TickableInfo> _queuedTasks = new List<TickableInfo>();

        public StandardKernel(
            [InjectOptional]
            List<ITickable> tickables,
            [InjectOptional]
            List<Tuple<Type, int>> priorities,
            DiContainer container)
        {
            var priorityMap = priorities.ToDictionary(x => x.First, x => x.Second);

            if (Assert.IsEnabled)
            {
                WarnForMissingBindings(tickables, container);
            }

            foreach (var tickable in tickables)
            {
                int priority;
                bool success = priorityMap.TryGetValue(tickable.GetType(), out priority);

                if (!success)
                {
                    //Log.Warn(
                        //String.Format("Tickable with type '{0}' does not have a tick priority assigned",
                        //tickable.GetType()));
                }

                var tickInfo = (success ? new TickableInfo(tickable, priority) : new TickableInfo(tickable));
                _queuedTasks.Add(tickInfo);
            }
        }

        void WarnForMissingBindings(List<ITickable> tickables, DiContainer container)
        {
            var boundTypes = tickables.Select(x => x.GetType()).Distinct();

            var unboundTypes = container.AllConcreteTypes.Where(x => x.DerivesFrom<ITickable>() && !boundTypes.Contains(x));

            foreach (var objType in unboundTypes)
            {
                Log.Warn("Found unbound ITickable with type '" + objType.Name() + "'");
            }
        }

        IEnumerable<TickableInfo> AllTasks
        {
            get
            {
                return ActiveTasks.Concat(_queuedTasks);
            }
        }

        IEnumerable<TickableInfo> ActiveTasks
        {
            get
            {
                return _sortedTasks.Concat(_unsortedTasks);
            }
        }

        public void AddTask(ITickable task)
        {
            AddTaskInternal(task, null);
        }

        public void AddTask(ITickable task, int priority)
        {
            AddTaskInternal(task, priority);
        }

        void AddTaskInternal(ITickable task, int? priority)
        {
            Assert.That(!AllTasks.Select(x => x.Tickable).Contains(task),
                "Duplicate task added to kernel with name '" + task.GetType().FullName + "'");

            // Wait until next frame to add the task, otherwise whether it gets updated
            // on the current frame depends on where in the update order it was added
            // from, so you might get off by one frame issues
            _queuedTasks.Add(
                new TickableInfo(task, priority));
        }

        public void RemoveTask(ITickable task)
        {
            var info = _sortedTasks.Concat(_unsortedTasks).Where(x => x.Tickable == task).Single();

            Assert.That(!info.IsRemoved, "Tried to remove task twice, task = " + task.GetType().Name);
            info.IsRemoved = true;
        }

        public void OnFrameStart()
        {
            // See above comment
            AddQueuedTasks();
        }

        public void UpdateAll()
        {
            Update(int.MinValue, int.MaxValue);
        }

        public void Update(int minPriority, int maxPriority)
        {
            foreach (var taskInfo in _sortedTasks.Where(x =>
                !x.IsRemoved && x.Priority >= minPriority && x.Priority < maxPriority))
            {
                Assert.That(taskInfo.Priority.HasValue);

                using (ProfileBlock.Start("{0}.Tick()".With(taskInfo.Tickable.GetType().Name())))
                {
                    taskInfo.Tickable.Tick();
                }
            }

            ClearRemovedTasks(_sortedTasks);
        }

        public void UpdateUnsorted()
        {
            foreach (var taskInfo in _unsortedTasks.Where(x => !x.IsRemoved))
            {
                Assert.That(!taskInfo.Priority.HasValue);

                using (ProfileBlock.Start("{0}.Tick()".With(taskInfo.Tickable.GetType().Name())))
                {
                    taskInfo.Tickable.Tick();
                }
            }

            ClearRemovedTasks(_unsortedTasks);
        }

        void ClearRemovedTasks(LinkedList<TickableInfo> tasks)
        {
            var node = tasks.First;

            while (node != null)
            {
                var next = node.Next;
                var info = node.Value;

                if (info.IsRemoved)
                {
                    //Log.Warn("Removed task '" + info.Tickable.GetType().ToString() + "'");
                    tasks.Remove(node);
                }

                node = next;
            }
        }

        void AddQueuedTasks()
        {
            foreach (var task in _queuedTasks)
            {
                InsertTaskSorted(task);
            }
            _queuedTasks.Clear();
        }

        void InsertTaskSorted(TickableInfo task)
        {
            if (!task.Priority.HasValue)
            {
                _unsortedTasks.AddLast(task);
                return;
            }

            for (var current = _sortedTasks.First; current != null; current = current.Next)
            {
                Assert.That(current.Value.Priority.HasValue);

                if (current.Value.Priority > task.Priority)
                {
                    _sortedTasks.AddBefore(current, task);
                    return;
                }
            }

            _sortedTasks.AddLast(task);
        }

        class TickableInfo
        {
            public ITickable Tickable;
            public int? Priority;
            public bool IsRemoved;

            public TickableInfo(ITickable tickable, int? priority)
            {
                Tickable = tickable;
                Priority = priority;
            }

            public TickableInfo(ITickable tickable)
                : this(tickable, null)
            {
            }
        }
    }
}
