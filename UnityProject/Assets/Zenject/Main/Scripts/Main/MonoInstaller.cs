using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using UnityEngine;

namespace ModestTree.Zenject
{
    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        private DiContainer _container;

        [Inject]
        public DiContainer Container
        {
            set { _container = value; }
            protected get { return _container; }
        }

        public abstract void InstallBindings();

        public virtual IEnumerable<ZenjectResolveException> ValidateSubGraphs()
        {
            // optional
            return Enumerable.Empty<ZenjectResolveException>();
        }

        public virtual void Start()
        {
            // Define this method so we expose the enabled check box
        }

        /// <summary>
        ///     Helper method for ValidateSubGraphs
        /// </summary>
        protected IEnumerable<ZenjectResolveException> Validate<T>(params Type[] extraTypes)
        {
            return _container.ValidateObjectGraph<T>(extraTypes);
        }
    }
}