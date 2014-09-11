using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using ModestTree.Zenject.Api.Factories;

namespace ModestTree.Zenject
{
    // Instantiate given concrete class
    public class Factory<TContract, TConcrete> : IFactory<TContract> where TConcrete : TContract
    {
        readonly DiContainer _container;
        Instantiator _instantiator;

        public Factory(DiContainer container)
        {
            _container = container;
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            if (_instantiator == null)
            {
                _instantiator = _container.Resolve<Instantiator>();
            }

            return _instantiator.Instantiate<TConcrete>(constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            return _container.ValidateObjectGraph<TConcrete>(extras);
        }
    }

    // Instantiate given contract class
    public class Factory<TContract> : IFactory<TContract>
    {
        readonly DiContainer _container;
        readonly Type _concreteType;
        Instantiator _instantiator;

        [Inject]
        public Factory(DiContainer container)
        {
            _container = container;
            _concreteType = typeof(TContract);
        }

        public Factory(DiContainer container, Type concreteType)
        {
            if (!concreteType.DerivesFromOrEqual(typeof(TContract)))
            {
                throw new ZenjectResolveException(
                    "Expected type '{0}' to derive from '{1}'".With(concreteType.Name(), typeof(TContract).Name()));
            }

            _container = container;
            _concreteType = concreteType;
        }

        public virtual TContract Create(params object[] constructorArgs)
        {
            if (_instantiator == null)
            {
                _instantiator = _container.Resolve<Instantiator>();
            }

            return (TContract)_instantiator.Instantiate(_concreteType, constructorArgs);
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            return _container.ValidateObjectGraph(_concreteType, extras);
        }
    }
}
