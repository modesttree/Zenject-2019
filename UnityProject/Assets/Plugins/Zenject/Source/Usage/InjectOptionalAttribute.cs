using System;
using JetBrains.Annotations;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Parameter
        | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	[MeansImplicitUse]
    public class InjectOptionalAttribute : InjectAttributeBase
    {
        public InjectOptionalAttribute()
        {
            Optional = true;
        }
    }
}

