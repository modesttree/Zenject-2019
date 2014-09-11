using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject
{
    public class TransientProvider : ProviderBase
    {
        readonly Type _concreteType;
        readonly DiContainer _container;

        Instantiator _instantiator;

        public TransientProvider(DiContainer container, Type concreteType)
        {
            _container = container;
            _concreteType = concreteType;
        }

        public override Type GetInstanceType()
        {
            return _concreteType;
        }

        public override bool HasInstance(Type contractType)
        {
            return false;
        }

        public override object GetInstance(Type contractType, InjectContext context)
        {
            if (_instantiator == null)
            {
                _instantiator = _container.Resolve<Instantiator>();
            }

            var obj = _instantiator.Instantiate(GetTypeToInstantiate(contractType));
            Assert.That(obj != null);
            return obj;
        }

        Type GetTypeToInstantiate(Type contractType)
        {
            if (_concreteType.IsOpenGenericType())
            {
                Assert.That(!contractType.IsAbstract);
                Assert.That(contractType.GetGenericTypeDefinition() == _concreteType);
                return contractType;
            }

            Assert.That(_concreteType.DerivesFromOrEqual(contractType));
            return _concreteType;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(Type contractType, InjectContext context)
        {
            return BindingValidator.ValidateObjectGraph(_container, _concreteType);
        }
    }
}
