using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Misc;

namespace ModestTree.Zenject
{
    /// <summary>
    ///     Interface for kernel class.
    ///     Currently there is only one (UnityKernel) but there should be another eventually, once Zenject adds support for
    ///     non-unity projects.
    /// </summary>
    public interface IKernel
    {
        void AddTask(ITickable task);
        void AddTask(ITickable task, int priority);

        void RemoveTask(ITickable task);
    }
}