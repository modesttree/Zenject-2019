namespace Zenject.Tests.Installers.MonoInstallers
{
    public class QuxInstaller : MonoInstaller<string, float, int, QuxInstaller>
    {
        string _p1;

        [Inject]
        public void Construct(string p1, float p2, int p3)
        {
            _p1 = p1;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_p1);
        }
    }
}
