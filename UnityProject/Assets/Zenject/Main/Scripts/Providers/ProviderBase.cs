using System;
using System.Collections.Generic;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject
{
    public abstract class ProviderBase : IDisposable
    {
        object _identifier;
        BindingCondition _condition = delegate { return true; };

        public BindingCondition Condition
        {
            set
            {
                _condition = value;
            }
        }

        public object Identifier
        {
            get
            {
                return _identifier;
            }
            set
            {
                _identifier = value;
            }
        }

        public bool Matches(InjectContext context)
        {
            // Identifier will be null most of the time
            return IdentifiersMatch(context.Identifier) && _condition(context);
        }

        bool IdentifiersMatch(object identifier)
        {
            if (_identifier == null)
            {
                return identifier == null;
            }

            return _identifier.Equals(identifier);
        }

        // Return null if not applicable (for eg. if instance type is dependent on contractType)
        public abstract Type GetInstanceType();

        // Returns true if this provider already has an instance to return
        // and false in the case where the provider would create it next time
        // GetInstance is called
        // Is not applicable in some cases
        public abstract bool HasInstance(Type contractType);

        public abstract object GetInstance(Type contractType, InjectContext context);

        public abstract IEnumerable<ZenjectResolveException> ValidateBinding(Type contractType, InjectContext context);

        public virtual void Dispose()
        {
        }
    }
}
