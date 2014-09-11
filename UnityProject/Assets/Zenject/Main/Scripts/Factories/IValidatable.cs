using System.Collections.Generic;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject.Api.Factories
{
    public interface IValidatable
    {
        IEnumerable<ZenjectResolveException> Validate();
    }
}