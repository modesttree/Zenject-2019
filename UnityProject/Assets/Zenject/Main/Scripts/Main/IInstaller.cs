using System.Collections.Generic;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject
{
    /// <summary>
    ///     We extract the interface so that monobehaviours can be installers
    /// </summary>
    public interface IInstaller
    {
        DiContainer Container { set; }

        void InstallBindings();

        IEnumerable<ZenjectResolveException> ValidateSubGraphs();
    }
}