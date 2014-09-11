using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject
{
    // Iterate over fields/properties on a given object and inject any with the [Inject] attribute
    public class FieldsInjecter
    {
        public static void Inject(DiContainer container, object injectable)
        {
            Inject(container, injectable, Enumerable.Empty<object>());
        }

        public static void Inject(DiContainer container, object injectable, IEnumerable<object> additional)
        {
            Inject(container, injectable, additional, false);
        }

        public static void Inject(DiContainer container, object injectable, IEnumerable<object> additional, bool shouldUseAll)
        {
            Inject(container, injectable, additional, shouldUseAll, TypeAnalyzer.GetInfo(injectable.GetType()));
        }

        internal static void Inject(DiContainer container, object injectable, IEnumerable<object> additional, bool shouldUseAll, ZenjectTypeInfo typeInfo)
        {
            Assert.IsEqual(typeInfo.TypeAnalyzed, injectable.GetType());
            Assert.That(injectable != null);

            var additionalCopy = additional.ToList();

            foreach (var injectInfo in typeInfo.FieldInjectables.Concat(typeInfo.PropertyInjectables))
            {
                bool didInject = InjectFromExtras(injectInfo, injectable, additionalCopy);

                if (!didInject)
                {
                    InjectFromResolve(injectInfo, container, injectable);
                }
            }

            if (shouldUseAll && !additionalCopy.IsEmpty())
            {
                throw new ZenjectResolveException(
                    "Passed unnecessary parameters when injecting into type '{0}'. \nExtra Parameters: {1}\nObject graph:\n{2}"
                        .With(injectable.GetType().Name(), String.Join(",", additionalCopy.Select(x => x.GetType().Name()).ToArray()), DiContainer.GetCurrentObjectGraph()));
            }

            foreach (var methodInfo in typeInfo.PostInjectMethods)
            {
                using (ProfileBlock.Start("{0}.{1}()".With(injectable.GetType(), methodInfo.Name)))
                {
                    methodInfo.Invoke(injectable, new object[0]);
                }
            }
        }

        static bool InjectFromExtras(
            InjectableInfo injectInfo,
            object injectable, List<object> additional)
        {
            foreach (object obj in additional)
            {
                if (injectInfo.ContractType.IsAssignableFrom(obj.GetType()))
                {
                    Assert.IsNotNull(injectInfo.Setter);

                    injectInfo.Setter(injectable, obj);
                    additional.Remove(obj);
                    return true;
                }
            }

            return false;
        }

        static void InjectFromResolve(
            InjectableInfo injectInfo, DiContainer container, object targetInstance)
        {
            var valueObj = container.Resolve(injectInfo, targetInstance);

            if (valueObj == null && !container.AllowNullBindings)
            {
                // Do not change if optional
                // Since it may have some hard coded value
                Assert.That(injectInfo.Optional); // Should have thrown resolve exception otherwise
            }
            else
            {
                Assert.IsNotNull(injectInfo.Setter);
                injectInfo.Setter(targetInstance, valueObj);
            }
        }
    }
}

