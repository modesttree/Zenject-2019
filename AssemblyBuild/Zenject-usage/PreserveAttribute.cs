
using System;

namespace Zenject.Internal
{
    // We use PreserveAttribute here so that IL2CPP will not strip constructors/methods
    // that are explicitly marked [Inject]
    public class PreserveAttribute : Attribute
    {
    }
}

