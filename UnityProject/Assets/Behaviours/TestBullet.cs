using UnityEngine;
using Zenject;

namespace Behaviours
{
    public class TestBullet : ZenjectBehaviour
    {
        [Inject] private TestManager _manager;

        protected override void Awake()
        {
            base.Awake();

            Debug.Log(_manager.Foo);
        }
    }
}