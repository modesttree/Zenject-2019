#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModestTree.Util
{
    public enum MouseWheelScrollDirections
    {
        None,
        Up,
        Down,
    }

    public static class UnityUtil
    {
        static Func<KeyCode, bool> s_GetKey = Input.GetKey;
        static Func<KeyCode, bool> s_GetKeyDown = Input.GetKeyDown;
        static Func<string, float> s_GetAxis = Input.GetAxis;
        static Func<int, bool> s_GetMouseButtonUp = Input.GetMouseButtonUp;
        static Func<int, bool> s_GetMouseButtonDown = Input.GetMouseButtonDown;

        public static Func<KeyCode, bool> GetKey
        {
            get
            {
                return s_GetKey;
            }
            set
            {
                s_GetKey = value ?? Input.GetKey;
            }
        }

        public static Func<KeyCode, bool> GetKeyDown
        {
            get
            {
                return s_GetKeyDown;
            }
            set
            {
                s_GetKeyDown = value ?? Input.GetKeyDown;
            }
        }

        public static Func<string, float> GetAxis
        {
            get
            {
                return s_GetAxis;
            }
            set
            {
                s_GetAxis = value ?? Input.GetAxis;
            }
        }

        public static Func<int, bool> GetMouseButtonUp
        {
            get
            {
                return s_GetMouseButtonUp;
            }
            set
            {
                s_GetMouseButtonUp = value ?? Input.GetMouseButtonUp;
            }
        }

        public static Func<int, bool> GetMouseButtonDown
        {
            get
            {
                return s_GetMouseButtonDown;
            }
            set
            {
                s_GetMouseButtonDown = value ?? Input.GetMouseButtonDown;
            }
        }

        public static bool IsAltKeyDown
        {
            get
            {
                return GetKey(KeyCode.LeftAlt) || GetKey(KeyCode.RightAlt);
            }
        }

        public static bool IsControlKeyDown
        {
            get
            {
                return GetKey(KeyCode.LeftControl) || GetKey(KeyCode.RightControl);
            }
        }

        public static bool IsShiftKeyDown
        {
            get
            {
                return GetKey(KeyCode.LeftShift) || GetKey(KeyCode.RightShift);
            }
        }

        public static bool WasShiftKeyJustPressed
        {
            get
            {
                return GetKeyDown(KeyCode.LeftShift) || GetKeyDown(KeyCode.RightShift);
            }
        }

        public static bool WasAltKeyJustPressed
        {
            get
            {
                return GetKeyDown(KeyCode.LeftAlt) || GetKeyDown(KeyCode.RightAlt);
            }
        }

        public static MouseWheelScrollDirections CheckMouseScrollWheel()
        {
            var value = GetAxis("Mouse ScrollWheel");

            if (Mathf.Approximately(value, 0.0f))
            {
                return MouseWheelScrollDirections.None;
            }

            if (value < 0)
            {
                return MouseWheelScrollDirections.Down;
            }

            return MouseWheelScrollDirections.Up;
        }

        static int GetDepthLevel(Transform transform)
        {
            if (transform == null)
            {
                return 0;
            }

            return 1 + GetDepthLevel(transform.parent);
        }

        public static IEnumerable<T> GetComponentsInChildrenTopDown<T>(GameObject gameObject, bool includeInactive)
            where T : Component
        {
            return gameObject.GetComponentsInChildren<T>(includeInactive)
                .OrderBy(x =>
                    x == null ? int.MinValue : GetDepthLevel(x.transform));
        }

        public static IEnumerable<T> GetComponentsInChildrenBottomUp<T>(GameObject gameObject, bool includeInactive)
            where T : Component
        {
            return gameObject.GetComponentsInChildren<T>(includeInactive)
                .OrderByDescending(x =>
                    x == null ? int.MinValue : GetDepthLevel(x.transform));
        }

        public static List<GameObject> GetRootGameObjects()
        {
            return GameObject.FindObjectsOfType<Transform>().Where(x => x.parent == null).Select(x => x.gameObject).ToList();
        }

        // Returns more intuitive defaults
        // eg. An empty string rather than null
        // An empty collection (eg. List<>) rather than null
        public static object GetSmartDefaultValue(Type type)
        {
            if (type == typeof(string))
            {
                return "";
            }
            else if (type == typeof(Quaternion))
            {
                return Quaternion.identity;
            }
            else if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(List<>) || genericType == typeof(Dictionary<,>))
                {
                    return Activator.CreateInstance(type);
                }
            }

            return type.GetDefaultValue();
        }
    }
}
#endif
