using System;
using System.Collections.Generic;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using ModestTree.Zenject.Api.Factories;

namespace ModestTree.Zenject
{
    public abstract class FactoryCustom<TValue> : IValidatable
    {
        [Inject]
        readonly DiContainer _container = null;

        protected TValue Instantiate(params object[] constructorArgs)
        {
            return _container.Instantiate<TValue>(constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(GetDynamicParams());
        }

        protected abstract Type[] GetDynamicParams();
    }
}
