using System;
using ModestTree;
using ModestTree.Util;
#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public class UntypedBinder : TypeBinder
    {
        public UntypedBinder(
            DiContainer container, Type contractType,
            string identifier, SingletonProviderMap singletonMap)
            : base(container, contractType, identifier, singletonMap)
        {
        }

        public BindingConditionSetter ToLookup<TConcrete>()
        {
            return ToLookupBase<TConcrete>(null);
        }

        public BindingConditionSetter ToLookup<TConcrete>(string identifier)
        {
            return ToLookupBase<TConcrete>(identifier);
        }

        public BindingConditionSetter ToMethod(Type returnType, Func<InjectContext, object> method)
        {
            if (!returnType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(returnType, ContractType.Name()));
            }

            return ToProvider(new MethodProviderUntyped(returnType, method));
        }

        public BindingConditionSetter ToMethod<TContract>(Func<InjectContext, TContract> method)
        {
            return ToMethodBase<TContract>(method);
        }

        public BindingConditionSetter ToGetter<TObj, TContract>(Func<TObj, TContract> method)
        {
            return ToGetterBase<TObj, TContract>(null, method);
        }

        public BindingConditionSetter ToGetter<TObj, TContract>(string identifier, Func<TObj, TContract> method)
        {
            return ToGetterBase<TObj, TContract>(identifier, method);
        }

        public BindingConditionSetter ToTransient<TConcrete>()
        {
            return ToTransient(typeof(TConcrete));
        }

        public BindingConditionSetter ToSingle<TConcrete>(string concreteIdentifier)
        {
            return ToSingle(typeof(TConcrete), concreteIdentifier);
        }

        public BindingConditionSetter ToInstance<TConcrete>(TConcrete instance)
        {
            return ToInstance(typeof(TConcrete), instance);
        }

        public BindingConditionSetter ToSingleInstance<TConcrete>(TConcrete instance)
        {
            return ToSingleInstance(instance != null ? instance.GetType() : typeof(TConcrete), null, instance);
        }

        public BindingConditionSetter ToSingleInstance<TConcrete>(string concreteIdentifier, TConcrete instance)
        {
            return ToSingleInstance(instance != null ? instance.GetType() : typeof(TConcrete), concreteIdentifier, instance);
        }

        public BindingConditionSetter ToSingleMethod<TConcrete>(string concreteIdentifier, Func<InjectContext, TConcrete> method)
        {
            return ToSingleMethodBase<TConcrete>(concreteIdentifier, method);
        }

        public BindingConditionSetter ToSingleMethod<TConcrete>(Func<InjectContext, TConcrete> method)
        {
            return ToSingleMethodBase<TConcrete>(null, method);
        }

        public BindingConditionSetter ToSingle<TConcrete>()
        {
            return ToSingle(typeof(TConcrete), null);
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToResource<TConcrete>(string resourcePath)
        {
            return ToResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientPrefabResource<TConcrete>(string resourcePath)
        {
            return ToTransientPrefabResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientPrefab<TConcrete>(GameObject prefab)
        {
            return ToTransientPrefab(typeof(TConcrete), prefab);
        }

        // Creates a new game object and adds the given type as a new component on it
        // NOTE! The string given here is just a name and not a singleton identifier
        public BindingConditionSetter ToSingleGameObject<TConcrete>(string name)
            where TConcrete : Component
        {
            return ToSingleGameObject(typeof(TConcrete), name);
        }

        public BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(GameObject gameObject)
        {
            return ToSingleMonoBehaviourBase<TConcrete>(gameObject);
        }

        public BindingConditionSetter ToSinglePrefab<TConcrete>(GameObject prefab)
        {
            return ToSinglePrefab(typeof(TConcrete), null, prefab);
        }

        public BindingConditionSetter ToSinglePrefab<TConcrete>(string identifier, GameObject prefab)
        {
            return ToSinglePrefab(typeof(TConcrete), identifier, prefab);
        }

        public BindingConditionSetter ToSinglePrefabResource<TConcrete>(string resourcePath)
        {
            return ToSinglePrefabResource(typeof(TConcrete), null, resourcePath);
        }

        public BindingConditionSetter ToSinglePrefabResource<TConcrete>(string identifier, string resourcePath)
        {
            return ToSinglePrefabResource(typeof(TConcrete), identifier, resourcePath);
        }
#endif
    }
}
