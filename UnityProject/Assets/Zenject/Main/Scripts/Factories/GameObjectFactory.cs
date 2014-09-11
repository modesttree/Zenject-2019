using System;
using System.Collections.Generic;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using ModestTree.Zenject.Api.Factories;
using UnityEngine;

namespace ModestTree.Zenject
{
    // Instantiate via prefab
    public class GameObjectFactory<TContract, TConcrete> : IFactory<TContract> where TConcrete : Component, TContract
    {
        DiContainer _container;
        GameObject _prefab;
        GameObjectInstantiator _instantiator;

        public GameObjectFactory(DiContainer container, GameObject prefab)
        {
            if (!container.AllowNullBindings && prefab == null)
            {
                throw new ZenjectBindException(
                    "Null prefab given for binding with type '{0}'".With(typeof(TContract)));
            }

            _prefab = prefab;
            _container = container;
        }

        public TContract Create(params object[] constructorArgs)
        {
            if (_instantiator == null)
            {
                _instantiator = _container.Resolve<GameObjectInstantiator>();
            }

            var gameObj = _instantiator.Instantiate(_prefab, constructorArgs);

            var component = gameObj.GetComponentInChildren<TConcrete>();

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component '{0}' when creating game object from prefab".With(typeof(TConcrete).Name()));
            }

            return component;
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            return _container.ValidateObjectGraph<TConcrete>(extras);
        }
    }

    // Instantiate via prefab
    public class GameObjectFactory<TContract> : IFactory<TContract> where TContract : Component
    {
        DiContainer _container;
        GameObject _prefab;
        GameObjectInstantiator _instantiator;

        public GameObjectFactory(DiContainer container, GameObject prefab)
        {
            if (!container.AllowNullBindings && UnityUtil.IsNull(prefab))
            {
                throw new ZenjectBindException(
                    "Null prefab given for binding with type '{0}'".With(typeof(TContract)));
            }

            _prefab = prefab;
            _container = container;
        }

        public TContract Create(params object[] constructorArgs)
        {
            if (_instantiator == null)
            {
                _instantiator = _container.Resolve<GameObjectInstantiator>();
            }

            var gameObj = _instantiator.Instantiate(_prefab, constructorArgs);

            var component = gameObj.GetComponentInChildren<TContract>();

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component '{0}' when creating game object from prefab".With(typeof(TContract).Name()));
            }

            return component;
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extras)
        {
            return _container.ValidateObjectGraph<TContract>(extras);
        }
    }
}
