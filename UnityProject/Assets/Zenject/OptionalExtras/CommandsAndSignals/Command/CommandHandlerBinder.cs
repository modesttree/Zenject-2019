using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class CommandHandlerBinder
    {
        readonly BindFinalizerWrapper _finalizerWrapper;
        readonly Type _commandType;
        readonly DiContainer _container;

        public CommandHandlerBinder(
            DiContainer container, Type commandType, BindFinalizerWrapper finalizerWrapper)
        {
            _container = container;
            _commandType = commandType;
            _finalizerWrapper = finalizerWrapper;
        }

        public FromBinder By<THandler>(Func<THandler, Action> methodGetter)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            var lookupId = Guid.NewGuid();
            var lazyLookup = new Lazy<THandler>(
                _container, new InjectContext(_container, typeof(THandler), lookupId));

            _container.Bind(typeof(IDisposable)).To<InstanceMethodCommandHandler<THandler>>().AsCached()
                .WithArguments(_commandType, methodGetter, lazyLookup);

            return _container.Bind<THandler>().WithId(lookupId).To<THandler>();
        }

        public void ByMethod(Action method)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            _container.Bind(typeof(IDisposable)).To<StaticMethodCommandHandler>().AsCached()
                .WithArguments(_commandType, method);
        }
    }

    public class CommandHandlerBinder<TParam1>
    {
        readonly BindFinalizerWrapper _finalizerWrapper;
        readonly Type _commandType;
        readonly DiContainer _container;

        public CommandHandlerBinder(
            DiContainer container, Type commandType, BindFinalizerWrapper finalizerWrapper)
        {
            _container = container;
            _commandType = commandType;
            _finalizerWrapper = finalizerWrapper;
        }

        public FromBinder By<THandler>(Func<THandler, Action<TParam1>> methodGetter)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            var lookupId = Guid.NewGuid();
            var lazyLookup = new Lazy<THandler>(
                _container, new InjectContext(_container, typeof(THandler), lookupId));

            _container.Bind(typeof(IDisposable)).To<InstanceMethodCommandHandler<TParam1, THandler>>().AsCached()
                .WithArguments(_commandType, methodGetter, lazyLookup).NonLazy();

            return _container.Bind<THandler>().WithId(lookupId).To<THandler>();
        }

        public void ByMethod(Action<TParam1> method)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            _container.Bind(typeof(IDisposable)).To<StaticMethodCommandHandler<TParam1>>().AsCached()
                .WithArguments(_commandType, method);
        }
    }

    public class CommandHandlerBinder<TParam1, TParam2>
    {
        readonly BindFinalizerWrapper _finalizerWrapper;
        readonly Type _commandType;
        readonly DiContainer _container;

        public CommandHandlerBinder(
            DiContainer container, Type commandType, BindFinalizerWrapper finalizerWrapper)
        {
            _container = container;
            _commandType = commandType;
            _finalizerWrapper = finalizerWrapper;
        }

        public FromBinder By<THandler>(Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            var lookupId = Guid.NewGuid();
            var lazyLookup = new Lazy<THandler>(
                _container, new InjectContext(_container, typeof(THandler), lookupId));

            _container.Bind(typeof(IDisposable)).To<InstanceMethodCommandHandler<TParam1, TParam2, THandler>>().AsCached()
                .WithArguments(_commandType, methodGetter, lazyLookup).NonLazy();

            return _container.Bind<THandler>().WithId(lookupId).To<THandler>();
        }

        public void ByMethod(Action<TParam1, TParam2> method)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            _container.Bind(typeof(IDisposable)).To<StaticMethodCommandHandler<TParam1, TParam2>>().AsCached()
                .WithArguments(_commandType, method);
        }
    }

    public class CommandHandlerBinder<TParam1, TParam2, TParam3>
    {
        readonly BindFinalizerWrapper _finalizerWrapper;
        readonly Type _commandType;
        readonly DiContainer _container;

        public CommandHandlerBinder(
            DiContainer container, Type commandType, BindFinalizerWrapper finalizerWrapper)
        {
            _container = container;
            _commandType = commandType;
            _finalizerWrapper = finalizerWrapper;
        }

        public FromBinder By<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            var lookupId = Guid.NewGuid();
            var lazyLookup = new Lazy<THandler>(
                _container, new InjectContext(_container, typeof(THandler), lookupId));

            _container.Bind(typeof(IDisposable)).To<InstanceMethodCommandHandler<TParam1, TParam2, TParam3, THandler>>().AsCached()
                .WithArguments(_commandType, methodGetter, lazyLookup).NonLazy();

            return _container.Bind<THandler>().WithId(lookupId).To<THandler>();
        }

        public void ByMethod(Action<TParam1, TParam2, TParam3> method)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            _container.Bind(typeof(IDisposable)).To<StaticMethodCommandHandler<TParam1, TParam2, TParam3>>().AsCached()
                .WithArguments(_commandType, method);
        }
    }

    public class CommandHandlerBinder<TParam1, TParam2, TParam3, TParam4>
    {
        readonly BindFinalizerWrapper _finalizerWrapper;
        readonly Type _commandType;
        readonly DiContainer _container;

        public CommandHandlerBinder(
            DiContainer container, Type commandType, BindFinalizerWrapper finalizerWrapper)
        {
            _container = container;
            _commandType = commandType;
            _finalizerWrapper = finalizerWrapper;
        }

        public FromBinder By<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            var lookupId = Guid.NewGuid();
            var lazyLookup = new Lazy<THandler>(
                _container, new InjectContext(_container, typeof(THandler), lookupId));

            _container.Bind(typeof(IDisposable)).To<InstanceMethodCommandHandler<TParam1, TParam2, TParam3, TParam4, THandler>>().AsCached()
                .WithArguments(_commandType, methodGetter, lazyLookup).NonLazy();

            return _container.Bind<THandler>().WithId(lookupId).To<THandler>();
        }

        public void ByMethod(Action<TParam1, TParam2, TParam3, TParam4> method)
        {
            // This is just to ensure they don't stop at ImplementCommand
            _finalizerWrapper.SubFinalizer = new NullBindingFinalizer();

            _container.Bind(typeof(IDisposable)).To<StaticMethodCommandHandler<TParam1, TParam2, TParam3, TParam4>>().AsCached()
                .WithArguments(_commandType, method);
        }
    }
}
