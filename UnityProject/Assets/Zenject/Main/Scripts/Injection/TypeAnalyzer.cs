using System;
using System.Collections.Generic;
using System.Reflection;
using ModestTree.Zenject.Api.Misc;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    internal static class TypeAnalyzer
    {
        static Dictionary<Type, ZenjectTypeInfo> _typeInfo = new Dictionary<Type, ZenjectTypeInfo>();

        public static ZenjectTypeInfo GetInfo(Type type)
        {
            ZenjectTypeInfo info;

            if (!_typeInfo.TryGetValue(type, out info))
            {
                info = CreateTypeInfo(type);
                _typeInfo.Add(type, info);
            }

            return info;
        }

        static ZenjectTypeInfo CreateTypeInfo(Type type)
        {
            var constructor = GetInjectConstructor(type);

            return new ZenjectTypeInfo(
                type,
                GetPostInjectMethods(type).ToList(),
                constructor,
                GetFieldInjectables(type).ToList(),
                GetPropertyInjectables(type).ToList(),
                GetConstructorInjectables(type, constructor).ToList());
        }

        static IEnumerable<InjectableInfo> GetConstructorInjectables(Type enclosingType, ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null)
            {
                return Enumerable.Empty<InjectableInfo>();
            }

            return constructorInfo.GetParameters().Select(
                paramInfo => CreateForConstructorParam(enclosingType, paramInfo));
        }

        static InjectableInfo CreateForConstructorParam(
            Type enclosingType, ParameterInfo paramInfo)
        {
            var injectAttr = paramInfo.AllAttributes<InjectAttribute>().FirstOrDefault();

            return new InjectableInfo()
            {
                Optional = paramInfo.HasAttribute(typeof(InjectOptionalAttribute)),
                Identifier = (injectAttr == null ? null : injectAttr.Identifier),
                SourceName = paramInfo.Name,
                ContractType = paramInfo.ParameterType,
                EnclosingType = enclosingType,
            };
        }

        static IEnumerable<MethodInfo> GetPostInjectMethods(Type type)
        {
            // Note that unlike with fields and properties we use GetCustomAttributes
            // This is so that we can ignore inherited attributes, which is necessary
            // otherwise a base class method marked with [Inject] would cause all overridden
            // derived methods to be added as well
            var methods = type.GetAllMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(PostInjectAttribute), false).Any()).ToList();

            var heirarchyList = type.Yield().Concat(type.GetParentTypes()).Reverse().ToList();

            // Order by base classes first
            // This is how constructors work so it makes more sense
            return methods.OrderBy(x => heirarchyList.IndexOf(x.DeclaringType));
        }

        static IEnumerable<InjectableInfo> GetPropertyInjectables(Type type)
        {
            var propInfos = type.GetAllProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.HasAttribute(typeof(InjectAttribute), typeof(InjectOptionalAttribute)));

            foreach (var propInfo in propInfos)
            {
                yield return CreateForMember(propInfo, type);
            }
        }

        static IEnumerable<InjectableInfo> GetFieldInjectables(Type type)
        {
            var fieldInfos = type.GetAllFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.HasAttribute(typeof(InjectAttribute), typeof(InjectOptionalAttribute)));

            foreach (var fieldInfo in fieldInfos)
            {
                yield return CreateForMember(fieldInfo, type);
            }
        }

        static InjectableInfo CreateForMember(MemberInfo memInfo, Type enclosingType)
        {
            var injectAttr = memInfo.AllAttributes<InjectAttribute>().FirstOrDefault();

            var info = new InjectableInfo()
            {
                Optional = memInfo.HasAttribute(typeof(InjectOptionalAttribute)),
                Identifier = (injectAttr == null ? null : injectAttr.Identifier),
                SourceName = memInfo.Name,
                EnclosingType = enclosingType,
            };

            if (memInfo is FieldInfo)
            {
                var fieldInfo = (FieldInfo)memInfo;
                info.Setter = ((object injectable, object value) => fieldInfo.SetValue(injectable, value));
                info.ContractType = fieldInfo.FieldType;
            }
            else
            {
                Assert.That(memInfo is PropertyInfo);
                var propInfo = (PropertyInfo)memInfo;
                info.Setter = ((object injectable, object value) => propInfo.SetValue(injectable, value, null));
                info.ContractType = propInfo.PropertyType;
            }

            return info;
        }

        static ConstructorInfo GetInjectConstructor(Type enclosingType)
        {
            var constructors = enclosingType.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (constructors.IsEmpty())
            {
                return null;
            }

            if (constructors.HasMoreThan(1))
            {
                // This will return null if there is more than one constructor and none are marked with the [Inject] attribute
                return (from c in constructors where c.HasAttribute<InjectAttribute>() select c).SingleOrDefault();
            }

            return constructors[0];
        }
    }

    internal class ZenjectTypeInfo
    {
        readonly List<MethodInfo> _postInjectMethods;
        readonly List<InjectableInfo> _constructorInjectables;
        readonly List<InjectableInfo> _fieldInjectables;
        readonly List<InjectableInfo> _propertyInjectables;
        readonly ConstructorInfo _injectConstructor;
        readonly Type _typeAnalyzed;

        public ZenjectTypeInfo(
            Type typeAnalyzed,
            List<MethodInfo> postInjectMethods,
            ConstructorInfo injectConstructor,
            List<InjectableInfo> fieldInjectables,
            List<InjectableInfo> propertyInjectables,
            List<InjectableInfo> constructorInjectables)
        {
            _postInjectMethods = postInjectMethods;
            _fieldInjectables = fieldInjectables;
            _propertyInjectables = propertyInjectables;
            _constructorInjectables = constructorInjectables;
            _injectConstructor = injectConstructor;
            _typeAnalyzed = typeAnalyzed;
        }

        public Type TypeAnalyzed
        {
            get
            {
                return _typeAnalyzed;
            }
        }

        public IEnumerable<MethodInfo> PostInjectMethods
        {
            get
            {
                return _postInjectMethods;
            }
        }

        public IEnumerable<InjectableInfo> AllInjectables
        {
            get
            {
                return _constructorInjectables.Concat(_fieldInjectables).Concat(_propertyInjectables);
            }
        }

        public IEnumerable<InjectableInfo> FieldInjectables
        {
            get
            {
                return _fieldInjectables;
            }
        }

        public IEnumerable<InjectableInfo> PropertyInjectables
        {
            get
            {
                return _propertyInjectables;
            }
        }

        public IEnumerable<InjectableInfo> ConstructorInjectables
        {
            get
            {
                return _constructorInjectables;
            }
        }

        // May be null
        public ConstructorInfo InjectConstructor
        {
            get
            {
                return _injectConstructor;
            }
        }
    }
}
