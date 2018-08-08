using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

namespace Zenject.Internal
{
    public interface IDecoratorProvider
    {
        List<object> GetAllInstances(
            IProvider provider, InjectContext context);
    }

    public class DecoratorProvider<TContract> : IDecoratorProvider
    {
        readonly Dictionary<IProvider, List<object>> _cachedInstances =
            new Dictionary<IProvider, List<object>>();

        readonly DiContainer _container;
        readonly List<Guid> _factoryBindIds = new List<Guid>();

        List<IFactory<TContract, TContract>> _decoratorFactories;

        public DecoratorProvider(DiContainer container)
        {
            _container = container;
        }

        public void AddFactoryId(Guid factoryBindId)
        {
            _factoryBindIds.Add(factoryBindId);
        }

        void LazyInitializeDecoratorFactories()
        {
            if (_decoratorFactories == null)
            {
                _decoratorFactories = _factoryBindIds.Select(
                    x => _container.ResolveId<IFactory<TContract, TContract>>(x)).ToList();
            }
        }

        public List<object> GetAllInstances(
            IProvider provider, InjectContext context)
        {
            List<object> instances;

            if (provider.IsCached)
            {
                if (!_cachedInstances.TryGetValue(provider, out instances))
                {
                    instances = WrapProviderInstances(provider, context);
                    _cachedInstances.Add(provider, instances);
                }
            }
            else
            {
                instances = WrapProviderInstances(provider, context);
            }

            return instances;
        }

        List<object> WrapProviderInstances(IProvider provider, InjectContext context)
        {
            LazyInitializeDecoratorFactories();

            var rawInstances = provider.GetAllInstances(context);
            var decoratedInstances = new List<object>(rawInstances.Count);

            if (context.Container.IsValidating)
            {
                for (int i = 0; i < rawInstances.Count; i++)
                {
                    for (int j = 0; j < _decoratorFactories.Count; j++)
                    {
                        decoratedInstances.Add(
                            _decoratorFactories[j].Create(default(TContract)));
                    }
                }

                return decoratedInstances;
            }

            for (int i = 0; i < rawInstances.Count; i++)
            {
                decoratedInstances.Add(DecorateInstance(
                    (TContract)rawInstances[i]));
            }

            return decoratedInstances;
        }

        TContract DecorateInstance(TContract instance)
        {
            for (int i = 0; i < _decoratorFactories.Count; i++)
            {
                instance = _decoratorFactories[i].Create(instance);
            }

            return instance;
        }
    }
}
