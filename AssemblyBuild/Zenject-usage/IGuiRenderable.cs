namespace Zenject
{
    // Note that if you want to bind to this class to have GuiRender called on
    // your non-MonoBehaviour classes, you also need to add the following to an installer
    // somewhere
    //
    // `Container.Bind<GuiRenderer>().FromNewComponentOnNewGameObject().NonLazy()` to an installer somewhere
    //
    // Or, if you always want this to be supported in all scenes, then add the following inside
    // an installer that is attached to the ProjectContext:
    //
    // `Container.Bind<GuiRenderer>().FromNewComponentOnNewGameObject().AsSingle().MoveIntoDirectSubContainers().NonLazy()`
    //
    // You also need to add this to a high level installer like ProjectContext:
    //
    // Container.Bind<GuiRenderableManager>().AsSingle().CopyIntoAllSubContainers();
    // 
    // We could add the contents of GuiRenderer into MonoKernel, but this adds
    // undesirable per-frame allocations.
    //
    public interface IGuiRenderable
    {
        void GuiRender();
    }
}

