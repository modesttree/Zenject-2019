using System;
using System.Reflection;
using Zenject.Internal;

namespace Zenject
{
    // An injectable is a field or property with [Inject] attribute
    // Or a constructor parameter
    [NoReflectionBaking]
    public class InjectableInfo
    {
        public readonly bool Optional;
        public readonly object Identifier;

        public readonly InjectSources SourceType;

        // The field name or property name from source code
        public readonly string MemberName;
        // The field type or property type from source code
        public readonly Type MemberType;

        public readonly object DefaultValue;

        public InjectableInfo(
            bool optional, object identifier, string memberName, Type memberType,
            object defaultValue, InjectSources sourceType)
        {
            Optional = optional;
            MemberType = memberType;
            MemberName = memberName;
            Identifier = identifier;
            DefaultValue = defaultValue;
            SourceType = sourceType;
        }

    }
}
