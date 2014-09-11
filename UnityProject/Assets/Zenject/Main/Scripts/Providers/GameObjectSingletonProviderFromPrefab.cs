using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using ModestTree.Zenject.Api.Factories;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class GameObjectSingletonProviderFromPrefab<T> : ProviderBase where T : Component
    {
        IFactory<T> _factory;
        object _instance;
        DiContainer _container;

        public GameObjectSingletonProviderFromPrefab(DiContainer container, GameObject template)
        {
            _factory = new GameObjectFactory<T>(container, template);
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override bool HasInstance(Type contractType)
        {
            Assert.That(typeof(T).DerivesFromOrEqual(contractType));
            return _instance != null;
        }

        public override object GetInstance(Type contractType, InjectContext context)
        {
            Assert.That(typeof(T).DerivesFromOrEqual(contractType));

            if (_instance == null)
            {
                _instance = _factory.Create();
                Assert.That(_instance != null);
            }

            return _instance;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(Type contractType, InjectContext context)
        {
            return BindingValidator.ValidateObjectGraph(_container, typeof(T));
        }
    }
}
