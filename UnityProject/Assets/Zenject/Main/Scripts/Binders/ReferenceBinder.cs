using System;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class ReferenceBinder<TContract> : BinderGeneric<TContract> where TContract : class
    {
        readonly protected SingletonProviderMap _singletonMap;

        public ReferenceBinder(DiContainer container, SingletonProviderMap singletonMap)
            : base(container)
        {
            _singletonMap = singletonMap;
        }

        public BindingConditionSetter ToTransient()
        {
            if (_contractType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToTransient for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToTransientFromPrefab"
                    .With(_contractType.Name()));
            }

            return ToProvider(new TransientProvider(_container, typeof(TContract)));
        }

        public BindingConditionSetter ToTransient<TConcrete>() where TConcrete : TContract
        {
            if (typeof(TConcrete).DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToTransient for Monobehaviours (when binding type '{0}' to '{1}'), you probably want either ToLookup or ToTransientFromPrefab"
                    .With(_contractType.Name(), typeof(TConcrete).Name()));
            }

            return ToProvider(new TransientProvider(_container, typeof(TConcrete)));
        }

        public BindingConditionSetter ToSingle()
        {
            if (_contractType.DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToSingle for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToSingleFromPrefab or ToSingleGameObject"
                    .With(_contractType.Name()));
            }

            return ToProvider(_singletonMap.CreateProvider<TContract>());
        }

        public BindingConditionSetter ToSingle<TConcrete>() where TConcrete : TContract
        {
            if (typeof(TConcrete).DerivesFrom(typeof(MonoBehaviour)))
            {
                throw new ZenjectBindException(
                    "Should not use ToSingle for Monobehaviours (when binding type '{0}' to '{1}'), you probably want either ToLookup or ToSingleFromPrefab or ToSingleGameObject"
                    .With(_contractType.Name(), typeof(TConcrete).Name()));
            }

            return ToProvider(_singletonMap.CreateProvider<TConcrete>());
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

        public BindingConditionSetter To<TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (_contractType == typeof(TConcrete))
                {
                    message = "Received null instance during Bind command with type '{0}'".With(_contractType.Name());
                }
                else
                {
                    message =
                        "Received null instance during Bind command when binding type '{0}' to '{1}'".With(_contractType.Name(), typeof(TConcrete).Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(new InstanceProvider(typeof(TConcrete), instance));
        }

        public BindingConditionSetter ToSingle<TConcrete>(TConcrete instance) where TConcrete : TContract
        {
            if (UnityUtil.IsNull(instance) && !_container.AllowNullBindings)
            {
                string message;

                if (_contractType == typeof(TConcrete))
                {
                    message = "Received null singleton instance during Bind command with type '{0}'".With(_contractType.Name());
                }
                else
                {
                    message =
                        "Received null singleton instance during Bind command when binding type '{0}' to '{1}'".With(_contractType.Name(), typeof(TConcrete).Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(_singletonMap.CreateProvider(instance));
        }

        // we can't have this method because of the necessary where() below, so in this case they have to specify TContract twice
        //public BindingConditionSetter ToSingle(GameObject prefab)

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToSingleFromPrefab<TConcrete>(GameObject prefab) where TConcrete : Component, TContract
        {
            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'".With(typeof(TConcrete).Name()));
            }

            return ToProvider(new GameObjectSingletonProviderFromPrefab<TConcrete>(_container, prefab));
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientFromPrefab<TConcrete>(GameObject prefab) where TConcrete : Component, TContract
        {
            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (UnityUtil.IsNull(prefab) && !_container.AllowNullBindings)
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'".With(typeof(TConcrete).Name()));
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
        public BindingConditionSetter ToSingleGameObject<TConcrete>(string name) where TConcrete : MonoBehaviour, TContract
        {
            return ToProvider(new GameObjectSingletonProvider(typeof(TConcrete), _container, name));
        }
    }
}
