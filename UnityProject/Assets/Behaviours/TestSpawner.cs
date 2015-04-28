using JetBrains.Annotations;
using UnityEngine;

namespace Behaviours
{
    public class TestSpawner : MonoBehaviour
    {
        [SerializeField] private TestBullet _testBulletPrefab;

        [UsedImplicitly]
        private void Awake()
        {
            Instantiate(_testBulletPrefab);
        }
    }
}