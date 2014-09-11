using System;
using System.Collections.Generic;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using ModestTree.Zenject.Api.Factories;

namespace ModestTree.Zenject
{
    // Instantiate using a delegate
    public class FactoryMethod<TContract> : IFactory<TContract>
    {
        readonly DiContainer _container;
        readonly Func<DiContainer, object[], TContract> _method;

        public FactoryMethod(DiContainer container, Func<DiContainer, object[], TContract> method)
        {
            _container = container;
            _method = method;
        }

        public TContract Create(params object[] constructorArgs)
        {
            return _method(_container, constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            yield break;
        }
    }
}
