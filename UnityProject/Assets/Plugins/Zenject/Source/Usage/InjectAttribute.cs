using System;

using JetBrains.Annotations;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Constructor
        | AttributeTargets.Method | AttributeTargets.Parameter
        | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    [MeansImplicitUse]
    public class InjectAttribute : InjectAttributeBase
    {
    }
}

