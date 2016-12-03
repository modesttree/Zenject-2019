using System;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class ConditionBinder : CopyIntoSubContainersBinder
    {
        public ConditionBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public CopyIntoSubContainersBinder When(BindingCondition condition)
        {
            BindInfo.Condition = condition;
            return this;
        }

        public CopyIntoSubContainersBinder WhenInjectedIntoInstance(object instance)
        {
            return When(r => ReferenceEquals(r.ObjectInstance, instance));
        }

        public CopyIntoSubContainersBinder WhenInjectedInto(params Type[] targets)
        {
            return When(r => targets.Where(x => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(x)).Any());
        }

        public CopyIntoSubContainersBinder WhenInjectedInto<T>()
        {
            return When(r => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(typeof(T)));
        }

        public CopyIntoSubContainersBinder WhenNotInjectedInto<T>()
        {
            return When(r => r.ObjectType == null || !r.ObjectType.DerivesFromOrEqual(typeof(T)));
        }
    }
}
