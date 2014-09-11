using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject
{
    internal static class BindingValidator
    {
        public static IEnumerable<ZenjectResolveException> ValidateContract(
            DiContainer container, Type contractType)
        {
            return ValidateContract(
                container, contractType, new InjectContext(container));
        }

        public static IEnumerable<ZenjectResolveException> ValidateContract(
            DiContainer container, Type contractType, InjectContext context)
        {
            var matches = container.GetProviderMatches(contractType, context);

            if (matches.Count == 1)
            {
                foreach (var error in matches.Single().ValidateBinding(contractType, context))
                {
                    yield return error;
                }
            }
            else
            {
                if (ReflectionUtil.IsGenericList(contractType))
                {
                    var subType = contractType.GetGenericArguments().Single();

                    matches = container.GetProviderMatches(subType, context);

                    if (matches.IsEmpty())
                    {
                        if (!context.Optional)
                        {
                            if (container.FallbackProvider != null)
                            {
                                foreach (var error in container.FallbackProvider.ValidateBinding(contractType, context))
                                {
                                    yield return error;
                                }
                            }
                            else
                            {
                                yield return new ZenjectResolveException(
                                    "Could not find dependency with type 'List<{0}>'{1}.  If the empty list is also valid, you can allow this by using the [InjectOptional] attribute.' \nObject graph:\n{2}"
                                    .With(
                                        subType.Name(),
                                        (context.EnclosingType == null ? "" : " when injecting into '{0}'".With(context.EnclosingType.Name())),
                                        DiContainer.GetCurrentObjectGraph()));
                            }
                        }
                    }
                    else
                    {
                        foreach (var match in matches)
                        {
                            foreach (var error in match.ValidateBinding(contractType, context))
                            {
                                yield return error;
                            }
                        }
                    }
                }
                else
                {
                    if (!context.Optional)
                    {
                        if (matches.IsEmpty())
                        {
                            if (container.FallbackProvider != null)
                            {
                                foreach (var error in container.FallbackProvider.ValidateBinding(contractType, context))
                                {
                                    yield return error;
                                }
                            }
                            else
                            {
                                yield return new ZenjectResolveException(
                                    "Could not find required dependency with type '{0}'{1} \nObject graph:\n{2}"
                                    .With(
                                        contractType.Name(),
                                        (context.EnclosingType == null ? "" : " when injecting into '{0}'".With(context.EnclosingType.Name())),
                                        DiContainer.GetCurrentObjectGraph()));
                            }
                        }
                        else
                        {
                            yield return new ZenjectResolveException(
                                "Found multiple matches when only one was expected for dependency with type '{0}'{1} \nObject graph:\n{2}"
                                .With(
                                    contractType.Name(),
                                    (context.EnclosingType == null ? "" : " when injecting into '{0}'".With(context.EnclosingType.Name())),
                                    DiContainer.GetCurrentObjectGraph()));
                        }
                    }
                }
            }
        }

        public static IEnumerable<ZenjectResolveException> ValidateObjectGraph(
            DiContainer container, Type concreteType, params Type[] extras)
        {
            using (container.PushLookup(concreteType))
            {
                var typeInfo = TypeAnalyzer.GetInfo(concreteType);
                var extrasList = extras.ToList();

                foreach (var dependInfo in typeInfo.AllInjectables)
                {
                    Assert.IsEqual(dependInfo.EnclosingType, concreteType);

                    if (TryTakingFromExtras(dependInfo.ContractType, extrasList))
                    {
                        continue;
                    }

                    var context = new InjectContext(
                        container, dependInfo, DiContainer.LookupsInProgress.ToList(), null);

                    foreach (var error in ValidateContract(
                        container, dependInfo.ContractType, context))
                    {
                        yield return error;
                    }
                }

                if (!extrasList.IsEmpty())
                {
                    yield return new ZenjectResolveException(
                        "Found unnecessary extra parameters passed when injecting into '{0}' with types '{1}'.  \nObject graph:\n{2}"
                        .With(concreteType.Name(), String.Join(",", extrasList.Select(x => x.Name()).ToArray()), DiContainer.GetCurrentObjectGraph()));
                }
            }
        }

        static bool TryTakingFromExtras(Type contractType, List<Type> extrasList)
        {
            foreach (var extraType in extrasList)
            {
                if (extraType.DerivesFromOrEqual(contractType))
                {
                    var removed = extrasList.Remove(extraType);
                    Assert.That(removed);
                    return true;
                }
            }

            return false;
        }
    }
}
