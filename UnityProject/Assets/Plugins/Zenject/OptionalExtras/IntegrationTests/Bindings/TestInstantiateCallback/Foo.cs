using UnityEngine;

namespace Zenject.Tests.Bindings.InstantiateCallback
{
    public class Foo : MonoBehaviour
    {
        public string Value
        {
            get; set;
        }

        public bool WasInjected
        {
            get;
            private set;
        }

        [Inject]
        public void Construct()
        {
            WasInjected = true;
        }
    }
}
