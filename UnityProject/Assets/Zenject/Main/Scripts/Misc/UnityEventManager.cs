#if !ZEN_NOT_UNITY3D

using System;
using ModestTree.Util;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class UnityEventManager : MonoBehaviour, ITickable
    {
        public event Action ApplicationGainedFocus = delegate { };
        public event Action ApplicationLostFocus = delegate { };
        public event Action<bool> ApplicationFocusChanged = delegate { };

        public event Action ApplicationQuit = delegate { };
        public event Action ChangingScenes = delegate { };
        public event Action DrawGizmos = delegate { };

        public event Action<int> MouseButtonDown = delegate { };
        public event Action<int> MouseButtonUp = delegate { };

        public event Action LeftMouseButtonDown = delegate { };
        public event Action LeftMouseButtonUp = delegate { };

        public event Action MiddleMouseButtonDown = delegate { };
        public event Action MiddleMouseButtonUp = delegate { };

        public event Action RightMouseButtonDown = delegate { };
        public event Action RightMouseButtonUp = delegate { };

        public event Action<MouseWheelScrollDirections> MouseWheelMoved = delegate { };

        public event Action MouseMove = delegate { };

        public event Action ScreenSizeChanged = delegate { };

        public event Action Started = delegate { };

        Vector3 _lastMousePosition;

        int _lastWidth;
        int _lastHeight;

        public bool IsFocused
        {
            get;
            private set;
        }

        void Start()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            Started();
        }

        public void Tick()
        {
            int buttonLeft = 0;
            int buttonRight = 1;
            int buttonMiddle = 2;

            if (UnityUtil.GetMouseButtonDown(buttonLeft))
            {
                LeftMouseButtonDown();
                MouseButtonDown(0);
            }
            else if (UnityUtil.GetMouseButtonUp(buttonLeft))
            {
                LeftMouseButtonUp();
                MouseButtonUp(0);
            }

            if (UnityUtil.GetMouseButtonDown(buttonRight))
            {
                RightMouseButtonDown();
                MouseButtonDown(1);
            }
            else if (UnityUtil.GetMouseButtonUp(buttonRight))
            {
                RightMouseButtonUp();
                MouseButtonUp(1);
            }

            if (UnityUtil.GetMouseButtonDown(buttonMiddle))
            {
                MiddleMouseButtonDown();
                MouseButtonDown(2);
            }
            else if (UnityUtil.GetMouseButtonUp(buttonMiddle))
            {
                MiddleMouseButtonUp();
                MouseButtonUp(2);
            }

            if (_lastMousePosition != Input.mousePosition)
            {
                _lastMousePosition = Input.mousePosition;
                MouseMove();
            }

            var mouseWheelState = UnityUtil.CheckMouseScrollWheel();

            if (mouseWheelState != MouseWheelScrollDirections.None)
            {
                MouseWheelMoved(mouseWheelState);
            }

            if (_lastWidth != Screen.width || _lastHeight != Screen.height)
            {
                _lastWidth = Screen.width;
                _lastHeight = Screen.height;
                ScreenSizeChanged();
            }
        }

        void OnDestroy()
        {
            ChangingScenes();
        }

        void OnApplicationQuit()
        {
            ApplicationQuit();
        }

        void OnDrawGizmos()
        {
            DrawGizmos();
        }

        void OnApplicationFocus(bool newIsFocused)
        {
            if (newIsFocused && !IsFocused)
            {
                IsFocused = true;
                ApplicationGainedFocus();
                ApplicationFocusChanged(true);
            }

            if (!newIsFocused && IsFocused)
            {
                IsFocused = false;
                ApplicationLostFocus();
                ApplicationFocusChanged(false);
            }
        }
    }
}

#endif
