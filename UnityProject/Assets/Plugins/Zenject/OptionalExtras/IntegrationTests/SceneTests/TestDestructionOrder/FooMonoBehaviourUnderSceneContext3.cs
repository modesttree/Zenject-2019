using UnityEngine;

namespace Zenject.Tests.TestDestructionOrder
{
    public class FooMonoBehaviourUnderSceneContext3 : MonoBehaviour
    {
        public void OnDestroy()
        {
            Debug.Log("Destroyed FooMonoBehaviourUnderSceneContext3");
        }
    }
}
