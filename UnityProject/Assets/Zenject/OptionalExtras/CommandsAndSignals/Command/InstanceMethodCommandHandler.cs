using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public abstract class InstanceMethodCommandHandlerBase<THandler> : CommandHandlerBase
    {
        readonly Lazy<THandler> _handler;

        public InstanceMethodCommandHandlerBase(
            Type commandType, CommandManager manager,
            Lazy<THandler> handler)
            : base(commandType, manager)
        {
            _handler = handler;
        }

        public override void Execute(object[] args)
        {
            InternalExecute(_handler.Value, args);
        }

        protected abstract void InternalExecute(THandler handler, object[] args);
    }

    public class InstanceMethodCommandHandler<THandler> : InstanceMethodCommandHandlerBase<THandler>
    {
        readonly Func<THandler, Action> _methodGetter;

        public InstanceMethodCommandHandler(
            Type commandType, CommandManager manager, Lazy<THandler> handler,
            Func<THandler, Action> methodGetter)
            : base(commandType, manager, handler)
        {
            _methodGetter = methodGetter;
        }

        protected override void InternalExecute(THandler handler, object[] args)
        {
            Assert.That(args.IsEmpty());
            _methodGetter(handler)();
        }
    }

    public class InstanceMethodCommandHandler<TParam1, THandler> : InstanceMethodCommandHandlerBase<THandler>
    {
        readonly Func<THandler, Action<TParam1>> _methodGetter;

        public InstanceMethodCommandHandler(
            Type commandType, CommandManager manager, Lazy<THandler> handler,
            Func<THandler, Action<TParam1>> methodGetter)
            : base(commandType, manager, handler)
        {
            _methodGetter = methodGetter;
        }

        protected override void InternalExecute(THandler handler, object[] args)
        {
            Assert.That(args.IsLength(1));
            ValidateParameter<TParam1>(args[0]);
            _methodGetter(handler)((TParam1)args[0]);
        }
    }

    public class InstanceMethodCommandHandler<TParam1, TParam2, THandler> : InstanceMethodCommandHandlerBase<THandler>
    {
        readonly Func<THandler, Action<TParam1, TParam2>> _methodGetter;

        public InstanceMethodCommandHandler(
            Type commandType, CommandManager manager, Lazy<THandler> handler,
            Func<THandler, Action<TParam1, TParam2>> methodGetter)
            : base(commandType, manager, handler)
        {
            _methodGetter = methodGetter;
        }

        protected override void InternalExecute(THandler handler, object[] args)
        {
            Assert.That(args.IsLength(2));
            ValidateParameter<TParam1>(args[0]);
            ValidateParameter<TParam2>(args[1]);
            _methodGetter(handler)((TParam1)args[0], (TParam2)args[1]);
        }
    }

    public class InstanceMethodCommandHandler<TParam1, TParam2, TParam3, THandler> : InstanceMethodCommandHandlerBase<THandler>
    {
        readonly Func<THandler, Action<TParam1, TParam2, TParam3>> _methodGetter;

        public InstanceMethodCommandHandler(
            Type commandType, CommandManager manager, Lazy<THandler> handler,
            Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
            : base(commandType, manager, handler)
        {
            _methodGetter = methodGetter;
        }

        protected override void InternalExecute(THandler handler, object[] args)
        {
            Assert.That(args.IsLength(3));
            ValidateParameter<TParam1>(args[0]);
            ValidateParameter<TParam2>(args[1]);
            ValidateParameter<TParam3>(args[2]);
            _methodGetter(handler)((TParam1)args[0], (TParam2)args[1], (TParam3)args[2]);
        }
    }

    public class InstanceMethodCommandHandler<TParam1, TParam2, TParam3, TParam4, THandler> : InstanceMethodCommandHandlerBase<THandler>
    {
        readonly Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> _methodGetter;

        public InstanceMethodCommandHandler(
            Type commandType, CommandManager manager, Lazy<THandler> handler,
            Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
            : base(commandType, manager, handler)
        {
            _methodGetter = methodGetter;
        }

        protected override void InternalExecute(THandler handler, object[] args)
        {
            Assert.That(args.IsLength(4));
            ValidateParameter<TParam1>(args[0]);
            ValidateParameter<TParam2>(args[1]);
            ValidateParameter<TParam3>(args[2]);
            ValidateParameter<TParam4>(args[3]);
            _methodGetter(handler)((TParam1)args[0], (TParam2)args[1], (TParam3)args[2], (TParam4)args[3]);
        }
    }
}
