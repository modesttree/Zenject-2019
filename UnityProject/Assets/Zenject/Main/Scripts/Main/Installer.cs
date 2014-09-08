using System;
using System.Collections.Generic;
using System.Linq;

namespace ModestTree.Zenject
{
    public abstract class Installer : IInstaller
    {
        protected DiContainer Container;

        [Inject]
        public DiContainer DiContainer
        {
            set
            {
                Container = value;
            }
        }

        public abstract void InstallBindings();

        public virtual IEnumerable<ZenjectResolveException> ValidateSubGraphs()
        {
            // optional
            return Enumerable.Empty<ZenjectResolveException>();
        }

        // Helper method for ValidateSubGraphs
        protected IEnumerable<ZenjectResolveException> Validate<T>(params Type[] extraTypes)
        {
            return Container.ValidateObjectGraph<T>(extraTypes);
        }
    }
}
