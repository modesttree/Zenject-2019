using System;
using UnityEngine;

namespace Zenject.Tests.Bindings.FromPrefabResource
{
    public class Bob : MonoBehaviour
    {
        [NonSerialized]
        [Inject]
        public Jim Jim;
    }
}
