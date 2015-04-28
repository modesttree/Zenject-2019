using System;
using System.Linq;
using Zenject;
using Object = UnityEngine.Object;

namespace Behaviours
{
    public static class BehaviourInjector
    {
        private static DiContainer _instance;

        private static DiContainer Container
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var root = Object.FindObjectsOfType<CompositionRoot>().SingleOrDefault();

                if (root == null)
                    throw new NullReferenceException("There is no composition root in the scene.");

                _instance = root.Container;

                if (_instance != null)
                    return _instance;

                root.Awake();
                _instance = root.Container;
                return _instance;
            }
        }

        public static void InjectInto(ZenjectBehaviour behaviour)
        {
            Container.InjectGameObject(behaviour.gameObject, true, true);
        }
    }
}