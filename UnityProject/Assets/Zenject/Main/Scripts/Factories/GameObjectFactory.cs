#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public abstract class GameObjectFactory : IValidatable
    {
        [Inject]
        protected readonly DiContainer _container;

        [Inject]
        protected readonly GameObject _prefab;

        public abstract IEnumerable<ZenjectResolveException> Validate();
    }

    public class GameObjectFactory<TValue> : GameObjectFactory, IFactory<TValue>
        // We can't do this because of the way IFactoryBinder works
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create()
        {
            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), _prefab);
        }

        public virtual TValue Create(GameObject prefab)
        {
           Assert.That(prefab != null,
              "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), prefab);
        }

        public virtual TValue Create(string prefabResourceName)
        {
            Assert.That(!string.IsNullOrEmpty(prefabResourceName),
              "Null or empty prefab resource name given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return Create((GameObject)Resources.Load(prefabResourceName));
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>();
        }
    }

    // One parameter
    public class GameObjectFactory<TParam1, TValue> : GameObjectFactory, IFactory<TParam1, TValue>
        // We can't do this because of the way IFactoryBinder works
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param)
        {
            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), _prefab, param);
        }

        public virtual TValue Create(GameObject prefab, TParam1 param)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), prefab, param);
        }

        public virtual TValue Create(string prefabResourceName, TParam1 param)
        {
            Assert.That(!string.IsNullOrEmpty(prefabResourceName),
              "Null or empty prefab resource name given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return Create((GameObject)Resources.Load(prefabResourceName), param);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1));
        }
    }

    // Two parameters
    public class GameObjectFactory<TParam1, TParam2, TValue> : GameObjectFactory, IFactory<TParam1, TParam2, TValue>
        // We can't do this because of the way IFactoryBinder works
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param1, TParam2 param2)
        {
            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), _prefab, param1, param2);
        }

        public virtual TValue Create(GameObject prefab, TParam1 param, TParam2 param2)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), prefab, param, param2);
        }

        public virtual TValue Create(string prefabResourceName, TParam1 param, TParam2 param2)
        {
            Assert.That(!string.IsNullOrEmpty(prefabResourceName),
              "Null or empty prefab resource name given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return Create((GameObject)Resources.Load(prefabResourceName), param, param2);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1), typeof(TParam2));
        }
    }

    // Three parameters
    public class GameObjectFactory<TParam1, TParam2, TParam3, TValue> : GameObjectFactory, IFactory<TParam1, TParam2, TParam3, TValue>
        // We can't do this because of the way IFactoryBinder works
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), _prefab, param1, param2, param3);
        }

        public virtual TValue Create(GameObject prefab, TParam1 param, TParam2 param2, TParam3 param3)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), prefab, param, param2, param3);
        }

        public virtual TValue Create(string prefabResourceName, TParam1 param, TParam2 param2, TParam3 param3)
        {
            Assert.That(!string.IsNullOrEmpty(prefabResourceName),
              "Null or empty prefab resource name given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return Create((GameObject)Resources.Load(prefabResourceName), param, param2, param3);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1), typeof(TParam2), typeof(TParam3));
        }
    }

    // Four parameters
    public class GameObjectFactory<TParam1, TParam2, TParam3, TParam4, TValue> : GameObjectFactory, IFactory<TParam1, TParam2, TParam3, TParam4, TValue>
        // We can't do this because of the way IFactoryBinder works
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), _prefab, param1, param2, param3, param4);
        }

        public virtual TValue Create(GameObject prefab, TParam1 param, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return (TValue)_container.InstantiatePrefabForComponent(typeof(TValue), prefab, param, param2, param3, param4);
        }

        public virtual TValue Create(string prefabResourceName, TParam1 param, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            Assert.That(!string.IsNullOrEmpty(prefabResourceName),
              "Null or empty prefab resource name given to factory create method when instantiating object with type '{0}'.", typeof(TValue));

            return Create((GameObject)Resources.Load(prefabResourceName), param, param2, param3, param4);
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4));
        }
    }
}

#endif
