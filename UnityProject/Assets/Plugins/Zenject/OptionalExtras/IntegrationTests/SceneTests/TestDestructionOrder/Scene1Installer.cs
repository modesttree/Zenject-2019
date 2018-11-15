using UnityEngine.SceneManagement;

namespace Zenject.Tests.TestDestructionOrder
{
    public class Scene1Installer : MonoInstaller<Scene1Installer>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<FooDisposable1>().AsSingle();

            SceneManager.LoadScene("TestDestructionOrder2", LoadSceneMode.Additive);
        }
    }
}
