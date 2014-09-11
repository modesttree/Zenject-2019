using System;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class BinderUntyped : Binder
    {
        readonly protected SingletonProviderMap _singletonMap;

        public BinderUntyped(
            DiContainer container, Type contractType, SingletonProviderMap singletonMap)
            : base(container, contractType)
        {
            _singletonMap = singletonMap;
        }

        public BindingConditionSetter ToTransient(Type concreteType)
        {
            if (_contractType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToTransient for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToTransientFromPrefab"
                    .With(_contractType.Name()));
            }

            return ToProvider(new TransientProvider(_container, concreteType));
        }

        public BindingConditionSetter ToTransient()
        {
            if (_contractType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToTransient for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToTransientFromPrefab"
                    .With(_contractType.Name()));
            }

            return ToProvider(new TransientProvider(_container, _contractType));
        }

        public BindingConditionSetter ToSingle()
        {
            if (_contractType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToSingle for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToSingleFromPrefab or ToSingleGameObject"
                    .With(_contractType.Name()));
            }

            return ToProvider(_singletonMap.CreateProvider(_contractType));
        }

        public BindingConditionSetter ToSingle(Type concreteType)
        {
            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".With(concreteType.Name(), _contractType.Name()));
            }

            return ToProvider(_singletonMap.CreateProvider(concreteType));
        }

        public BindingConditionSetter To(object instance)
        {
            var concreteType = instance.GetType();

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".With(concreteType.Name(), _contractType.Name()));
            }

            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (_contractType == concreteType)
                {
                    message = "Received null instance during Bind command with type '{0}'".With(_contractType.Name());
                }
                else
                {
                    message =
                        "Received null instance during Bind command when binding type '{0}' to '{1}'".With(_contractType.Name(), concreteType.Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(new InstanceProvider(concreteType, instance));
        }

        public BindingConditionSetter ToSingle<TConcrete>(TConcrete instance)
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".With(concreteType.Name(), _contractType.Name()));
            }

            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (_contractType == concreteType)
                {
                    message = "Received null singleton instance during Bind command with type '{0}'".With(_contractType.Name());
                }
                else
                {
                    message =
                        "Received null singleton instance during Bind command when binding type '{0}' to '{1}'".With(_contractType.Name(), concreteType.Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(_singletonMap.CreateProvider(instance));
        }

        public BindingConditionSetter ToSingle<TConcrete>()
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".With(concreteType.Name(), _contractType.Name()));
            }

            if (concreteType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToSingle for Monobehaviours (when binding type '{0}' to '{1}'), you probably want either ToLookup or ToSingleFromPrefab or ToSingleGameObject"
                    .With(_contractType.Name(), concreteType.Name()));
            }

            return ToProvider(_singletonMap.CreateProvider<TConcrete>());
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToSingleFromPrefab<TConcrete>(GameObject prefab) where TConcrete : Component
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".With(concreteType.Name(), _contractType.Name()));
            }

            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'".With(concreteType.Name()));
            }

            return ToProvider(new GameObjectSingletonProviderFromPrefab<TConcrete>(_container, prefab));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientFromPrefab<TConcrete>(GameObject prefab) where TConcrete : Component
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".With(concreteType.Name(), _contractType.Name()));
            }

            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'".With(concreteType.Name()));
            }

            return ToProvider(new GameObjectTransientProviderFromPrefab<TConcrete>(_container, prefab));
        }

        public BindingConditionSetter ToSingleGameObject()
        {
            return ToSingleGameObject(_contractType.Name());
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject(string name)
        {
            if (!_contractType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException("Expected MonoBehaviour derived type when binding type '{0}'".With(_contractType.Name()));
            }

            return ToProvider(new GameObjectSingletonProvider(_contractType, _container, name));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject<TConcrete>(string name) where TConcrete : MonoBehaviour
        {
            var concreteType = typeof(TConcrete);

            if (!concreteType.DerivesFromOrEqual(_contractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".With(concreteType.Name(), _contractType.Name()));
            }

            return ToProvider(new GameObjectSingletonProvider(typeof(TConcrete), _container, name));
        }
    }
}
