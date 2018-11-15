namespace Zenject.Tests.Installers.Installers
{
    public class QuxInstaller : Installer<string, float, int, QuxInstaller>
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
