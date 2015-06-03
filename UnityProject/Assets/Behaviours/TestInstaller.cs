using UnityEngine;
using Zenject;

namespace Behaviours
{
    public class TestInstaller : MonoInstaller
    {
        [SerializeField] private TestManager _testManger;

        public override void InstallBindings()
        {
            Container.Bind<TestManager>().ToInstance(_testManger);
        }
    }
}