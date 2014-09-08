using System;
using System.Collections.Generic;
using ModestTree.Zenject;

namespace ModestTree.Zenject
{
    public class InitializablePrioritiesInstaller : Installer
    {
        readonly List<Type> _initializables;

        public InitializablePrioritiesInstaller(
            List<Type> initializables)
        {
            _initializables = initializables;
        }

        public override void InstallBindings()
        {
            int priorityCount = 1;

            foreach (var initializableType in _initializables)
            {
                BindPriority(Container, initializableType, priorityCount);
                priorityCount++;
            }
        }

        public static void BindPriority(
            DiContainer container, Type initializableType, int priorityCount)
        {
            Assert.That(initializableType.DerivesFrom<IInitializable>(),
                "Expected type '{0}' to derive from IInitializable", initializableType.Name());

            container.Bind<Tuple<Type, int>>().To(
                Tuple.New(initializableType, priorityCount)).WhenInjectedInto<InitializableHandler>();
        }
    }
}

