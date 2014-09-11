using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject
{
    // This provider can be used to create nested containers
    public class DiContainerProvider : ProviderBase
    {
        DiContainer _container;

        public DiContainerProvider(DiContainer container)
        {
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return null;
        }

        public override bool HasInstance(Type contractType)
        {
            return false;
        }

        public override object GetInstance(Type contractType, InjectContext context)
        {
            return _container.Resolve(contractType, context);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(Type contractType, InjectContext context)
        {
            return _container.ValidateResolve(contractType, context);
        }
    }
}
