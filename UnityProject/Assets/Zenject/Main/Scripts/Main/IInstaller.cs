using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // We extract the interface so that monobehaviours can be installers
    public interface IInstaller
    {
        DiContainer DiContainer
        {
            set;
        }

        void InstallBindings();

        IEnumerable<ZenjectResolveException> ValidateSubGraphs();
    }
}
