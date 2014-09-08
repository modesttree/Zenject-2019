using System;
using System.Collections.Generic;
using ModestTree.Zenject;

namespace ModestTree.Zenject
{
    public class DisposablePrioritiesInstaller : Installer
    {
        readonly List<Type> _disposables;

        public DisposablePrioritiesInstaller(List<Type> disposables)
        {
            _disposables = disposables;
        }

        public override void InstallBindings()
        {
            int priorityCount = 1;

            foreach (var disposableType in _disposables)
            {
                BindPriority(Container, disposableType, priorityCount);
                priorityCount++;
            }
        }

        public static void BindPriority(
            DiContainer container, Type disposableType, int priorityCount)
        {
            Assert.That(disposableType.DerivesFrom<IDisposable>(),
                "Expected type '{0}' to derive from IDisposable", disposableType.Name());

            container.Bind<Tuple<Type, int>>().To(
                Tuple.New(disposableType, priorityCount)).WhenInjectedInto<DisposablesHandler>();
        }
    }
}

