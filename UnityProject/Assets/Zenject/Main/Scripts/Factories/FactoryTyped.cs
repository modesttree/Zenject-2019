using System;
using System.Collections.Generic;
using ModestTree.Zenject.Api;
using ModestTree.Zenject.Api.Exceptions;
using ModestTree.Zenject.Api.Factories;
using ModestTree.Zenject.Api.Misc;

namespace ModestTree.Zenject
{
    public class FactoryTyped<TValue> : IFactoryTyped<TValue>
    {
        [Inject]
        readonly DiContainer _container = null;

        public TValue Create()
        {
            return _container.Instantiate<TValue>();
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>();
        }
    }

    public class FactoryTyped<TParam1, TValue>
        : IFactoryTyped<TParam1, TValue>
    {
        [Inject]
        readonly DiContainer _container = null;

        public TValue Create(TParam1 param)
        {
            return _container.Instantiate<TValue>(param);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1));
        }
    }

    public class FactoryTyped<TParam1, TParam2, TValue> : IFactoryTyped<TParam1, TParam2, TValue>
    {
        [Inject]
        readonly DiContainer _container = null;

        public TValue Create(TParam1 param1, TParam2 param2)
        {
            return _container.Instantiate<TValue>(param1, param2);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(
                typeof(TParam1), typeof(TParam2));
        }
    }

    public class FactoryTyped<TParam1, TParam2, TParam3, TValue>
        : IFactoryTyped<TParam1, TParam2, TParam3, TValue>
    {
        [Inject]
        readonly DiContainer _container = null;

        public TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return _container.Instantiate<TValue>(param1, param2, param3);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(
                typeof(TParam1), typeof(TParam2), typeof(TParam3));
        }
    }

    public class FactoryTyped<TParam1, TParam2, TParam3, TParam4, TValue>
        : IFactoryTyped<TParam1, TParam2, TParam3, TParam4, TValue>
    {
        [Inject]
        readonly DiContainer _container = null;

        public TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return _container.Instantiate<TValue>(param1, param2, param3, param4);
        }

        public IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(
                typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4));
        }
    }
}
