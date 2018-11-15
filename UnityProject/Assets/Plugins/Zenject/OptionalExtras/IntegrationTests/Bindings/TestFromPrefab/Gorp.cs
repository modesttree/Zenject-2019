using ModestTree;
using UnityEngine;

#pragma warning disable 649

namespace Zenject.Tests.Bindings.FromPrefab
{
    public class Gorp : MonoBehaviour
    {
        [Inject]
        string _arg;

        public string Arg
        {
            get { return _arg; }
        }

        [Inject]
        public void Initialize()
        {
            Log.Trace("Received arg '{0}' in Gorp", _arg);
        }
    }
}
