using System;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Misc;

namespace ModestTree.Zenject
{
    public static class KernelUtil
    {
        public static void BindTickable<TTickable>(DiContainer container, int priority) where TTickable : ITickable
        {
            container.Bind<ITickable>().ToSingle<TTickable>();
            BindTickablePriority<TTickable>(container, priority);
        }

        public static void BindTickablePriority<TTickable>(DiContainer container, int priority)
        {
            container.Bind<Tuple<Type, int>>().To(Tuple.New(typeof(TTickable), priority));
        }
    }
}
