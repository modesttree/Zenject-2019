using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public interface ICommandHandler
    {
        void Execute(object[] args);
    }
}
