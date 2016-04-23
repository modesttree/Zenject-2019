using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zenject;

namespace ModestTree
{
    public static class TypeExtensions
    {
        public static bool DerivesFrom<T>(this Type a)
        {
            return DerivesFrom(a, typeof(T));
        }

        // This seems easier to think about than IsAssignableFrom
        public static bool DerivesFrom(this Type a, Type b)
        {
            return b != a && b.IsAssignableFrom(a);
        }

        public static bool DerivesFromOrEqual<T>(this Type a)
        {
            return DerivesFromOrEqual(a, typeof(T));
        }

        public static bool DerivesFromOrEqual(this Type a, Type b)
        {
            return b == a || b.IsAssignableFrom(a);
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
#if USE_EXPRESSIONS
                return type.NewValueType();
#else
                return Activator.CreateInstance(type);
#endif
            }

            return null;
        }

        // Returns name without generic arguments
        public static string GetSimpleName(this Type type)
        {
            var name = type.Name;

            var quoteIndex = name.IndexOf("`");

            if (quoteIndex == -1)
            {
                return name;
            }

            // Remove the backtick
            return name.Substring(0, quoteIndex);
        }

        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            if (type == null || type.BaseType == null || type == typeof(object) || type.BaseType == typeof(object))
            {
                yield break;
            }

            yield return type.BaseType;

            foreach (var ancestor in type.BaseType.GetParentTypes())
            {
                yield return ancestor;
            }
        }

        public static string NameWithParents(this Type type)
        {
            var typeList = type.GetParentTypes().Prepend(type).Select(x => x.Name()).ToArray();
            return string.Join(":", typeList);
        }

        public static bool IsClosedGenericType(this Type type)
        {
            return type.IsGenericType && type != type.GetGenericTypeDefinition();
        }

        public static bool IsOpenGenericType(this Type type)
        {
            return type.IsGenericType && type == type.GetGenericTypeDefinition();
        }

        // This is the same as the standard GetFields except it also supports getting the private
        // fields in base classes
        public static IEnumerable<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
        {
            if ((int)(flags & BindingFlags.DeclaredOnly) != 0)
            {
                // Can use normal method in this case
                foreach (var fieldInfo in type.GetFields(flags))
                {
                    yield return fieldInfo;
                }
            }
            else
            {
                // Add DeclaredOnly because we will get the base classes below
                foreach (var fieldInfo in type.GetFields(flags | BindingFlags.DeclaredOnly))
                {
                    yield return fieldInfo;
                }

                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    foreach (var fieldInfo in type.BaseType.GetAllFields(flags))
                    {
                        yield return fieldInfo;
                    }
                }
            }
        }

        // This is the same as the standard GetProperties except it also supports getting the private
        // members in base classes
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type, BindingFlags flags)
        {
            if ((int)(flags & BindingFlags.DeclaredOnly) != 0)
            {
                // Can use normal method in this case
                foreach (var propertyInfo in type.GetProperties(flags))
                {
                    yield return propertyInfo;
                }
            }
            else
            {
                // Add DeclaredOnly because we will get the base classes below
                foreach (var propertyInfo in type.GetProperties(flags | BindingFlags.DeclaredOnly))
                {
                    yield return propertyInfo;
                }

                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    foreach (var propertyInfo in type.BaseType.GetAllProperties(flags))
                    {
                        yield return propertyInfo;
                    }
                }
            }
        }

        // This is the same as the standard GetMethods except it also supports getting the private
        // members in base classes
        public static IEnumerable<MethodInfo> GetAllMethods(this Type type, BindingFlags flags)
        {
            if ((int)(flags & BindingFlags.DeclaredOnly) != 0)
            {
                // Can use normal method in this case
                foreach (var methodInfo in type.GetMethods(flags))
                {
                    yield return methodInfo;
                }
            }
            else
            {
                // Add DeclaredOnly because we will get the base classes below
                foreach (var methodInfo in type.GetMethods(flags | BindingFlags.DeclaredOnly))
                {
                    yield return methodInfo;
                }

                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    foreach (var methodInfo in type.BaseType.GetAllMethods(flags))
                    {
                        yield return methodInfo;
                    }
                }
            }
        }

        public static string Name(this Type type)
        {
            if (type.IsArray)
            {
                return string.Format("{0}[]", type.GetElementType().Name());
            }

            if (type.ContainsGenericParameters || type.IsGenericType)
            {
                if (type.BaseType == typeof(Nullable<>) || (type.BaseType == typeof(ValueType) && type.UnderlyingSystemType.Name.StartsWith("Nullable")))
                {
                    return GetCSharpTypeName(type.GetGenericArguments().Single().Name) + "?";
                }

                int index = type.Name.IndexOf("`");
                string genericTypeName = index > 0 ? type.Name.Substring(0, index) : type.Name;
                string genericArgs = string.Join(",", type.GetGenericArguments().Select(t => t.Name()).ToArray());
                return genericArgs.Length == 0 ? genericTypeName : genericTypeName + "<" + genericArgs + ">";
            }

            // If a nested class, include the parent classes as well
            return (type.DeclaringType == null ? "" : type.DeclaringType.Name() + ".") + GetCSharpTypeName(type.Name);
        }

        static string GetCSharpTypeName(string typeName)
        {
            switch (typeName)
            {
                case "String":
                case "Object":
                case "Void":
                case "Byte":
                case "Double":
                case "Decimal":
                    return typeName.ToLower();
                case "Int16":
                    return "short";
                case "Int32":
                    return "int";
                case "Int64":
                    return "long";
                case "Single":
                    return "float";
                case "Boolean":
                    return "bool";
                default:
                    return typeName;
            }
        }

        public static bool HasAttribute(
            this ICustomAttributeProvider provider, params Type[] attributeTypes)
        {
            return provider.AllAttributes(attributeTypes).Any();
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider provider)
            where T : Attribute
        {
            return provider.AllAttributes(typeof(T)).Any();
        }

        public static IEnumerable<T> AllAttributes<T>(
            this ICustomAttributeProvider provider)
            where T : Attribute
        {
            return provider.AllAttributes(typeof(T)).Cast<T>();
        }

        public static IEnumerable<Attribute> AllAttributes(
            this ICustomAttributeProvider provider, params Type[] attributeTypes)
        {
            var allAttributes = provider.GetCustomAttributes(true).Cast<Attribute>();

            if (attributeTypes.Length == 0)
            {
                return allAttributes;
            }

            return allAttributes.Where(a => attributeTypes.Any(x => a.GetType().DerivesFromOrEqual(x)));
        }
    }
}
