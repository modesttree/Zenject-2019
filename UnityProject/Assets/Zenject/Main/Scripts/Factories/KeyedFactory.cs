using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using System.Linq;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject
{
    public class KeyedFactory<TBase, TKey>
    {
        readonly DiContainer _container;
        readonly Instantiator _instantiator;
        readonly Dictionary<TKey, Type> _typeMap;
        readonly Type _fallbackType;

        public KeyedFactory(
            List<Tuple<TKey, Type>> typePairs,
            Instantiator instantiator,
            [InjectOptional]
            Type fallbackType,
            DiContainer container)
        {
            Assert.That(fallbackType == null || fallbackType.DerivesFromOrEqual<TBase>(),
                "Expected fallback type '{0}' to derive from '{1}'", fallbackType, typeof(TBase));

            _typeMap = typePairs.ToDictionary(x => x.First, x => x.Second);
            _instantiator = instantiator;
            _container = container;
            _fallbackType = fallbackType;
        }

        public IEnumerable<ZenjectResolveException> ValidateExcept(
            IEnumerable<Type> ignoreTypes, params Type[] extraArgs)
        {
            foreach (var type in _typeMap.Values.Except(ignoreTypes))
            {
                foreach (var err in _container.ValidateObjectGraph(type, extraArgs))
                {
                    yield return err;
                }
            }
        }

        public IEnumerable<ZenjectResolveException> Validate(params Type[] extraArgs)
        {
            return ValidateExcept(Enumerable.Empty<Type>(), extraArgs);
        }

        public Type GetMapping(TKey key)
        {
            return _typeMap[key];
        }

        public TBase Create(TKey key, params object[] args)
        {
            Type desiredType;

            if (!_typeMap.TryGetValue(key, out desiredType))
            {
                Assert.IsNotNull(_fallbackType, "Could not find instance for key '{0}'", key);
                desiredType = _fallbackType;
            }

            return (TBase)_instantiator.Instantiate(desiredType, args);
        }

        public static IdentifierSetter AddBinding<TDerived>(DiContainer container, TKey key)
            where TDerived : TBase
        {
            return container.Bind<Tuple<TKey, Type>>().To(Tuple.New(key, typeof(TDerived))).WhenInjectedInto<KeyedFactory<TBase, TKey>>();
        }
    }
}
