using ModestTree;
using UnityEngine;

#pragma warning disable 649

namespace Zenject.Tests.Bindings.FromPrefabResource
{
    public class Qux : MonoBehaviour
    {
        [Inject]
        int _arg;

        [Inject]
        public void Initialize()
        {
            Log.Trace("Received arg '{0}' in Qux", _arg);
        }
    }
}
