using System;
using System.Linq;

namespace ModestTree.Zenject
{
    public class StandardUnityInstaller : Installer
    {
        // Install basic functionality for most unity apps
        public override void InstallBindings()
        {
            Container.Bind<UnityKernel>().ToSingleGameObject();

            Container.Bind<UnityEventManager>().ToSingleGameObject();
            Container.Bind<GameObjectInstantiator>().ToSingle();

            Container.Bind<StandardKernel>().ToSingle();
            // TODO: Do this instead:
            //_container.Bind<IKernel>().ToTransient<StandardKernel>();

            Container.Bind<InitializableHandler>().ToSingle();
            Container.Bind<DisposablesHandler>().ToSingle();
            Container.Bind<ITickable>().ToLookup<UnityEventManager>();
        }
    }
}
