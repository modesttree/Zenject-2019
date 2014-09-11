using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject
{
    public class Instantiator
    {
        readonly DiContainer _container;

        public Instantiator(DiContainer container)
        {
            _container = container;
        }

        public T Instantiate<T>(
            params object[] constructorArgs)
        {
            return (T)Instantiate(typeof(T), constructorArgs);
        }

        public object Instantiate(
            Type concreteType, params object[] constructorArgs)
        {
            using (ProfileBlock.Start("Zenject.Instantiate({0})".With(concreteType)))
            {
                using (_container.PushLookup(concreteType))
                {
                    return InstantiateInternal(concreteType, constructorArgs);
                }
            }
        }

        object InstantiateInternal(
            Type concreteType, params object[] constructorArgs)
        {
            Assert.That(!concreteType.DerivesFrom<UnityEngine.Component>(),
                "Error occurred while instantiating object of type '{0}'. Instantiator should not be used to create new mono behaviours.  Must use GameObjectInstantiator, GameObjectFactory, or GameObject.Instantiate.", concreteType.Name());

            var typeInfo = TypeAnalyzer.GetInfo(concreteType);

            if (typeInfo.InjectConstructor == null)
            {
                throw new ZenjectResolveException(
                    "More than one or zero constructors found for type '{0}' when creating dependencies.  Use one [Inject] attribute to specify which to use.".With(concreteType));
            }

            var paramValues = new List<object>();
            var extrasList = new List<object>(constructorArgs);

            Assert.That(!extrasList.Contains(null),
                "Null value given to factory constructor arguments. This is currently not allowed");

            foreach (var injectInfo in typeInfo.ConstructorInjectables)
            {
                var found = false;

                foreach (var extra in extrasList)
                {
                    if (extra.GetType().DerivesFromOrEqual(injectInfo.ContractType))
                    {
                        found = true;
                        paramValues.Add(extra);
                        extrasList.Remove(extra);
                        break;
                    }
                }

                if (!found)
                {
                    paramValues.Add(_container.Resolve(injectInfo));
                }
            }

            object newObj;

            try
            {
                using (ProfileBlock.Start("{0}.{0}()".With(concreteType)))
                {
                    newObj = typeInfo.InjectConstructor.Invoke(paramValues.ToArray());
                }
            }
            catch (Exception e)
            {
                throw new ZenjectResolveException(
                    "Error occurred while instantiating object with type '{0}'".With(concreteType.Name()), e);
            }

            FieldsInjecter.Inject(_container, newObj, extrasList, true, typeInfo);

            return newObj;
        }
    }
}
