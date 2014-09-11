using System;
using System.Collections.Generic;
using ModestTree.Zenject.Api.Exceptions;

namespace ModestTree.Zenject.Api.Factories
{
    /// <summary>
    ///     The difference between a factory and a provider:
    ///     Factories create new instances, providers might return an existing instance
    /// </summary>
    public interface IFactory<out T>
    {
        /// <summary>
        ///     Note that we lose some type safety here when passing the arguments.
        ///     We are trading compile time checks for some flexibility
        /// </summary>
        T Create(params object[] constructorArgs);

        IEnumerable<ZenjectResolveException> Validate(params Type[] extraType);
    }
}