using System;
using System.Collections.Generic;
using ModestTree.Zenject.Api.Misc;

namespace ModestTree.Zenject
{
    public class TickablePrioritiesInstaller : Installer
    {
        readonly List<Type> _tickables;

        public TickablePrioritiesInstaller(
            List<Type> tickables)
        {
            _tickables = tickables;
        }

        public override void InstallBindings()
        {
            int priorityCount = 1;

            foreach (var tickableType in _tickables)
            {
                BindPriority(Container, tickableType, priorityCount);
                priorityCount++;
            }
        }

        public static void BindPriority(
            DiContainer container, Type tickableType, int priorityCount)
        {
            Assert.That(tickableType.DerivesFrom<ITickable>(),
                "Expected type '{0}' to derive from ITickable", tickableType.Name());

            container.Bind<Tuple<Type, int>>().To(
                Tuple.New(tickableType, priorityCount)).WhenInjectedInto<StandardKernel>();
        }
    }
}
