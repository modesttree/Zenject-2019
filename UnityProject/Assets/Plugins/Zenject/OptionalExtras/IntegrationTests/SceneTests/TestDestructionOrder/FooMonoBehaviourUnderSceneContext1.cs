using UnityEngine;

namespace Zenject.Tests.TestDestructionOrder
{
    public class FooMonoBehaviourUnderSceneContext1 : MonoBehaviour
    {
        public void OnDestroy()
        {
            Debug.Log("Destroyed FooMonoBehaviourUnderSceneContext1");
        }
    }
}
