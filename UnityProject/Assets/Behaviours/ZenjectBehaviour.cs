using JetBrains.Annotations;
using UnityEngine;

namespace Behaviours
{
    public class ZenjectBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        protected virtual void Awake()
        {
            BehaviourInjector.InjectInto(this);
        }
    }
}