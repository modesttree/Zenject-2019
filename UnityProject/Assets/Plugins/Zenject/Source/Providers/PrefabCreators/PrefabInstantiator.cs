#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using Zenject.Internal;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    [NoReflectionBaking]
    public class PrefabInstantiator : IPrefabInstantiator
    {
        readonly IPrefabProvider _prefabProvider;
        readonly DiContainer _container;
        readonly List<TypeValuePair> _extraArguments;
        readonly GameObjectCreationParameters _gameObjectBindInfo;
        readonly Type _argumentTarget;
        readonly Action<InjectContext, object> _instantiateCallback;

        public PrefabInstantiator(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
            Type argumentTarget,
            IEnumerable<TypeValuePair> extraArguments,
            IPrefabProvider prefabProvider,
            Action<InjectContext, object> instantiateCallback)
        {
            _prefabProvider = prefabProvider;
            _extraArguments = extraArguments.ToList();
            _container = container;
            _gameObjectBindInfo = gameObjectBindInfo;
            _argumentTarget = argumentTarget;
            _instantiateCallback = instantiateCallback;
        }

        public GameObjectCreationParameters GameObjectCreationParameters
        {
            get { return _gameObjectBindInfo; }
        }

        public Type ArgumentTarget
        {
            get { return _argumentTarget; }
        }

        public List<TypeValuePair> ExtraArguments
        {
            get { return _extraArguments; }
        }

        public UnityEngine.Object GetPrefab()
        {
            return _prefabProvider.GetPrefab();
        }

        public GameObject Instantiate(List<TypeValuePair> args, out Action injectAction)
        {
            var context = new InjectContext(_container, _argumentTarget, null);
            bool shouldMakeActive;
            var gameObject = _container.CreateAndParentPrefab(
                GetPrefab(), _gameObjectBindInfo, context, out shouldMakeActive);
            Assert.IsNotNull(gameObject);

            injectAction = () =>
            {
                var allArgs = ZenPools.SpawnList<TypeValuePair>();

                allArgs.AllocFreeAddRange(_extraArguments);
                allArgs.AllocFreeAddRange(args);

                if (_argumentTarget == null)
                {
                    Assert.That(
                        allArgs.IsEmpty(),
                        "Unexpected arguments provided to prefab instantiator.  Arguments are not allowed if binding multiple components in the same binding");
                }

                Component targetComponent = null;

                if (_argumentTarget == null || allArgs.IsEmpty())
                {
                    _container.InjectGameObject(gameObject);
                }
                else
                {
                    targetComponent = _container.InjectGameObjectForComponentExplicit(
                        gameObject, _argumentTarget, allArgs, context, null);

                    Assert.That(allArgs.Count == 0);
                }

                ZenPools.DespawnList<TypeValuePair>(allArgs);

                if (shouldMakeActive && !_container.IsValidating)
                {
#if ZEN_INTERNAL_PROFILING
                    using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
                    {
                        gameObject.SetActive(true);
                    }
                }

                if (_instantiateCallback != null && _argumentTarget != null)
                {
                    if (targetComponent == null)
                    {
                        targetComponent = gameObject.GetComponentInChildren(_argumentTarget);
                    }

                    if (targetComponent != null)
                    {
                        _instantiateCallback(context, targetComponent);
                    }
                }
            };

            return gameObject;
        }
    }
}

#endif
