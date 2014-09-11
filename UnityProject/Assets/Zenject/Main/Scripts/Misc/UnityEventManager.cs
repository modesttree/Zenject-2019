using System;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Misc;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class UnityEventManager : MonoBehaviour, ITickable
    {
        public event Action ApplicationGainedFocus = delegate { };
        public event Action ApplicationLostFocus = delegate { };
        public event Action<bool> ApplicationFocusChanged = delegate { };

        public event Action ApplicationQuit = delegate { };
        public event Action ChangingScenes = delegate { };
        public event Action Gui = delegate { };
        public event Action DrawGizmos = delegate { };

        public event Action LeftMouseButtonDown = delegate { };
        public event Action LeftMouseButtonUp = delegate { };

        public event Action MiddleMouseButtonDown = delegate { };
        public event Action MiddleMouseButtonUp = delegate { };

        public event Action RightMouseButtonDown = delegate { };
        public event Action RightMouseButtonUp = delegate { };

        public event Action MouseMove = delegate { };

        public event Action ScreenSizeChanged = delegate { };

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
        }

        public void Tick()
        {
            int buttonLeft = 0;
            int buttonRight = 1;
            int buttonMiddle = 2;

            if (Input.GetMouseButtonDown(buttonLeft))
            {
                LeftMouseButtonDown();
            }
            else if (Input.GetMouseButtonUp(buttonLeft))
            {
                LeftMouseButtonUp();
            }

            if (Input.GetMouseButtonDown(buttonRight))
            {
                RightMouseButtonDown();
            }
            else if (Input.GetMouseButtonUp(buttonRight))
            {
                RightMouseButtonUp();
            }

            if (Input.GetMouseButtonDown(buttonMiddle))
            {
                MiddleMouseButtonDown();
            }
            else if (Input.GetMouseButtonUp(buttonMiddle))
            {
                MiddleMouseButtonUp();
            }

            if (_lastMousePosition != Input.mousePosition)
            {
                _lastMousePosition = Input.mousePosition;
                MouseMove();
            }

            if (_lastWidth != Screen.width || _lastHeight != Screen.height)
            {
                _lastWidth = Screen.width;
                _lastHeight = Screen.height;
                ScreenSizeChanged();
            }
        }

        void OnGUI()
        {
            Gui();
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
