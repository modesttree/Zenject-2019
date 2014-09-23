using System.Collections.Generic;
using ModestTree.Zenject.Api.Misc;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    // Implement the task kernel using Unity's
    // Update() function
    public class UnityKernel : MonoBehaviour
    {
        public const int LateUpdateTickPriority = 10000;
        public const int OnGuiTickPriority = 20000;

        [Inject]
        StandardKernel _impl = null;

        public void Update()
        {
            _impl.OnFrameStart();
            _impl.Update(int.MinValue, LateUpdateTickPriority);

            // Put Tickables that don't care about their priority after Update() and before LateUpdate()
            _impl.UpdateUnsorted();
        }

        public void LateUpdate()
        {
            _impl.Update(LateUpdateTickPriority, OnGuiTickPriority);
        }

        public void OnGUI()
        {
            _impl.Update(OnGuiTickPriority, int.MaxValue);
        }
    }
}
