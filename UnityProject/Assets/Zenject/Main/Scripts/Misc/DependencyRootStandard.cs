using System.Collections.Generic;
using ModestTree.Zenject.Api.Misc;

namespace ModestTree.Zenject
{
    public class DependencyRootStandard : IDependencyRoot
    {
        [Inject]
        public UnityKernel UnityKernel;

        [Inject]
        public InitializableHandler Initializer;

        [Inject]
        public DisposablesHandler DisposablesHandler;

        public virtual void Start()
        {
            Initializer.Initialize();
        }

        public virtual void Dispose()
        {
            DisposablesHandler.Dispose();
        }
    }

    public class DependencyRootStandard<TRoot> : DependencyRootStandard
        where TRoot : class
    {
        [Inject]
        public TRoot Root;
    }
}
