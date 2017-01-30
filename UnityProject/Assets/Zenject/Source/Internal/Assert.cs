using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace ModestTree
{
    public static class Assert
    {
        public static bool IsEnabled
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                // TODO: Compile out asserts for actual builds
                return false;
#endif
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void That(bool condition)
        {
            if (!condition)
            {
                Throw("Assert hit!");
            }
        }

        public static void DerivesFrom<T>(Type type)
        {
            if (!type.DerivesFrom<T>())
            {
                Throw("Expected type '{0}' to derive from '{1}'", type.Name, typeof(T).Name);
            }
        }

        public static void DerivesFromOrEqual<T>(Type type)
        {
            if (!type.DerivesFromOrEqual<T>())
            {
                Throw("Expected type '{0}' to derive from or be equal to '{1}'", type.Name, typeof(T).Name);
            }
        }

        public static void DerivesFromOrEqual(Type childType, Type parentType)
        {
            if (!childType.DerivesFromOrEqual(parentType))
            {
                Throw("Expected type '{0}' to derive from or be equal to '{1}'", childType.Name, parentType.Name);
            }
        }

        public static void IsEmpty<T>(IEnumerable<T> sequence)
        {
            if (!sequence.IsEmpty())
            {
                Throw("Expected collection to be empty but instead found '{0}' elements",
                    sequence.Count());
            }
        }

        public static void IsNotEmpty<T>(IEnumerable<T> val, string message = "")
        {
            if (!val.Any())
            {
                Throw("Assert Hit! Expected empty collection but found {0} values. {1}", val.Count(), message);
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsType<T>(object obj)
        {
            IsType<T>(obj, "");
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsType<T>(object obj, string message)
        {
            if (!(obj is T))
            {
                Throw("Assert Hit! Wrong type found. Expected '"+ typeof(T).Name + "' but found '" + obj.GetType().Name + "'. " + message);
            }
        }

        // Use AssertEquals to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right)
        {
            IsEqual(left, right, "");
        }

        public static void Throws(Action action)
        {
            Throws<Exception>(action);
        }

        //[Conditional("UNITY_EDITOR")]
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }

            Throw(string.Format("Expected to receive exception of type '{0}' but nothing was thrown", typeof(TException).Name));
        }

        // Use AssertEquals to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right, Func<string> messageGenerator)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                Throw("Assert Hit! Expected '" + right.ToString() + "' but found '" + left.ToString() + "'. " + messageGenerator());
            }
        }

        // Use AssertEquals to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right, string message)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                Throw("Assert Hit! Expected '" + right.ToString() + "' but found '" + left.ToString() + "'. " + message);
            }
        }

        // Use Assert.IsNotEqual to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsNotEqual(object left, object right)
        {
            IsNotEqual(left, right, "");
        }

        // Use Assert.IsNotEqual to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsNotEqual(object left, object right, Func<string> messageGenerator)
        {
            if(object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                Throw("Assert Hit! Expected '" + right.ToString() + "' to differ from '" + left.ToString() + "'. " + messageGenerator());
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsNull(object val)
        {
            if (val != null)
            {
                Throw("Assert Hit! Expected null pointer but instead found '" + val.ToString() + "'");
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val)
        {
            if (val == null)
            {
                Throw("Assert Hit! Found null pointer when value was expected");
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val, string message)
        {
            if (val == null)
            {
                Throw("Assert Hit! Found null pointer when value was expected. " + message);
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsNull(object val, string message, params object[] parameters)
        {
            if (val != null)
            {
                Throw("Assert Hit! Expected null pointer but instead found '" + val.ToString() + "': " + FormatString(message, parameters));
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val, string message, params object[] parameters)
        {
            if (val == null)
            {
                Throw("Assert Hit! Found null pointer when value was expected. " + FormatString(message, parameters));
            }
        }

        // Use Assert.IsNotEqual to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsNotEqual(object left, object right, string message)
        {
            if (object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                Throw("Assert Hit! Expected '" + right.ToString() + "' to differ from '" + left.ToString() + "'. " + message);
            }
        }

        // Pass a function instead of a string for cases that involve a lot of processing to generate a string
        // This way the processing only occurs when the assert fails
        //[Conditional("UNITY_EDITOR")]
        public static void That(bool condition, Func<string> messageGenerator)
        {
            if (!condition)
            {
                Throw("Assert hit! " + messageGenerator());
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void That(
            bool condition, string message, params object[] parameters)
        {
            if (!condition)
            {
                Throw("Assert hit! " + FormatString(message, parameters));
            }
        }

        public static void Warn(bool condition)
        {
            if (!condition)
            {
                Log.Warn("Warning!  See call stack");
            }
        }

        public static void Warn(bool condition, string message, params object[] parameters)
        {
            if (!condition)
            {
                Log.Warn("Warning Assert hit! " + FormatString(message, parameters));
            }
        }

        public static void Warn(bool condition, Func<string> messageGenerator)
        {
            if (!condition)
            {
                Log.Warn("Warning Assert hit! " + messageGenerator());
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void Throw()
        {
            Throw("Assert hit!");
        }

        public static void Throw(string message)
        {
            throw new ZenjectException(message);
        }

        public static void Throw(string message, params object[] parameters)
        {
            throw new ZenjectException(
                FormatString(message, parameters));
        }

        public static string FormatString(string format, params object[] parameters)
        {
            // doin this funky loop to ensure nulls are replaced with "NULL"
            // and that the original parameters array will not be modified
            if (parameters != null && parameters.Length > 0)
            {
                object[] paramToUse = parameters;

                foreach (object cur in parameters)
                {
                    if (cur == null)
                    {
                        paramToUse = new object[parameters.Length];

                        for (int i = 0; i < parameters.Length; ++i)
                        {
                            paramToUse[i] = parameters[i] ?? "NULL";
                        }

                        break;
                    }
                }

                format = string.Format(format, paramToUse);
            }

            return format;
        }

        public static ZenjectException CreateException()
        {
            return new ZenjectException("Assert hit!");
        }

        public static ZenjectException CreateException(string message)
        {
            return new ZenjectException(message);
        }

        public static ZenjectException CreateException(string message, params object[] parameters)
        {
            return new ZenjectException(FormatString(message, parameters));
        }

        public static ZenjectException CreateException(Exception innerException, string message, params object[] parameters)
        {
            return new ZenjectException(FormatString(message, parameters), innerException);
        }
    }
}
