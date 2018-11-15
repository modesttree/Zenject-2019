using System;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NoReflectionBakingAttribute : Attribute
    {
    }
}
