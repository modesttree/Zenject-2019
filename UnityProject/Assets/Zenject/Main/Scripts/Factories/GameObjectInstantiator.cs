using System;
using System.Collections.Generic;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using UnityEngine;

namespace ModestTree.Zenject
{
    // Helper to instantiate game objects and also inject
    // any dependencies they have
    public class GameObjectInstantiator
    {
        public event Action<GameObject> GameObjectInstantiated = delegate { };

        readonly DiContainer _container;
        readonly CompositionRoot _compRoot;

        public GameObjectInstantiator(DiContainer container)
        {
            _container = container;
            _compRoot = container.Resolve<CompositionRoot>();
        }

        public Transform RootParent
        {
            get
            {
                return _compRoot.transform;
            }
        }

        // Add a monobehaviour to an existing game object
        // Note: gameobject here is not a prefab prototype, it is an instance
        public TContract AddMonobehaviour<TContract>(GameObject gameObject, params object[] args) where TContract : MonoBehaviour
        {
            return (TContract)AddMonobehaviour(typeof(TContract), gameObject, args);
        }

        // Add a monobehaviour to an existing game object, using Type rather than a generic
        // Note: gameobject here is not a prefab prototype, it is an instance
        public MonoBehaviour AddMonobehaviour(
            Type behaviourType, GameObject gameObject, params object[] args)
        {
            Assert.That(behaviourType.DerivesFrom<MonoBehaviour>());
            var monoBehaviour = (MonoBehaviour)gameObject.AddComponent(behaviourType);
            InjectionHelper.InjectMonoBehaviour(_container, monoBehaviour, args);
            return monoBehaviour;
        }

        // Create a new game object from a given prefab
        // Without returning any particular monobehaviour
        public GameObject Instantiate(GameObject template, params object[] args)
        {
            var gameObj = (GameObject)GameObject.Instantiate(template);

            // By default parent to comp root
            // This is good so that the entire object graph is
            // contained underneath it, which is useful for cases
            // where you need to delete the entire object graph
            gameObj.transform.parent = _compRoot.transform;

            gameObj.SetActive(true);

            InjectionHelper.InjectChildGameObjects(_container, gameObj, args);

            GameObjectInstantiated(gameObj);

            return gameObj;
        }

        // Create from prefab and customize name
        // Return specific monobehaviour
        public T Instantiate<T>(GameObject template, string name) where T : Component
        {
            var component = Instantiate<T>(template);
            component.gameObject.name = name;
            return component;
        }

        // Create from prefab
        // Return specific monobehaviour
        public T Instantiate<T>(GameObject template) where T : Component
        {
            Assert.That(template != null, "Null template found when instantiating game object");

            var obj = Instantiate(template);

            var component = obj.GetComponentInChildren<T>();

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component with type '{0}' when instantiating template".With(typeof(T)));
            }

            return component;
        }

        public object Instantiate(Type type, string name)
        {
            var gameObj = new GameObject(name);
            gameObj.transform.parent = _compRoot.transform;

            var component = gameObj.AddComponent(type);

            if (type.DerivesFrom(typeof(MonoBehaviour)))
            {
                InjectionHelper.InjectMonoBehaviour(_container, (MonoBehaviour)component);
            }

            GameObjectInstantiated(gameObj);

            return component;
        }

        public T Instantiate<T>(string name) where T : Component
        {
            return (T)Instantiate(typeof(T), name);
        }

        public GameObject Instantiate(string name)
        {
            var gameObj = new GameObject(name);
            gameObj.transform.parent = _compRoot.transform;

            GameObjectInstantiated(gameObj);

            return gameObj;
        }
    }
}
