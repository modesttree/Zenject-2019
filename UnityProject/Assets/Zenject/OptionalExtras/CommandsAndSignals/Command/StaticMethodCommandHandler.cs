using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class StaticMethodCommandHandler : CommandHandlerBase
    {
        readonly Action _method;

        public StaticMethodCommandHandler(
            Type commandType, CommandManager manager, Action method)
            : base(commandType, manager)
        {
            _method = method;
        }

        public override void Execute(object[] args)
        {
            Assert.That(args.IsEmpty());
            _method();
        }
    }

    public class StaticMethodCommandHandler<TParam1> : CommandHandlerBase
    {
        readonly Action<TParam1> _method;

        public StaticMethodCommandHandler(
            Type commandType, CommandManager manager, Action<TParam1> method)
            : base(commandType, manager)
        {
            _method = method;
        }

        public override void Execute(object[] args)
        {
            Assert.That(args.IsLength(1));
            ValidateParameter<TParam1>(args[0]);
            _method((TParam1)args[0]);
        }
    }

    public class StaticMethodCommandHandler<TParam1, TParam2> : CommandHandlerBase
    {
        readonly Action<TParam1, TParam2> _method;

        public StaticMethodCommandHandler(
            Type commandType, CommandManager manager, Action<TParam1, TParam2> method)
            : base(commandType, manager)
        {
            _method = method;
        }

        public override void Execute(object[] args)
        {
            Assert.That(args.IsLength(1));
            ValidateParameter<TParam1>(args[0]);
            ValidateParameter<TParam2>(args[1]);
            _method((TParam1)args[0], (TParam2)args[1]);
        }
    }

    public class StaticMethodCommandHandler<TParam1, TParam2, TParam3> : CommandHandlerBase
    {
        readonly Action<TParam1, TParam2, TParam3> _method;

        public StaticMethodCommandHandler(
            Type commandType, CommandManager manager, Action<TParam1, TParam2, TParam3> method)
            : base(commandType, manager)
        {
            _method = method;
        }

        public override void Execute(object[] args)
        {
            Assert.That(args.IsLength(1));
            ValidateParameter<TParam1>(args[0]);
            ValidateParameter<TParam2>(args[1]);
            ValidateParameter<TParam3>(args[2]);
            _method((TParam1)args[0], (TParam2)args[1], (TParam3)args[2]);
        }
    }

    public class StaticMethodCommandHandler<TParam1, TParam2, TParam3, TParam4> : CommandHandlerBase
    {
        readonly Action<TParam1, TParam2, TParam3, TParam4> _method;

        public StaticMethodCommandHandler(
            Type commandType, CommandManager manager, Action<TParam1, TParam2, TParam3, TParam4> method)
            : base(commandType, manager)
        {
            _method = method;
        }

        public override void Execute(object[] args)
        {
            Assert.That(args.IsLength(1));
            ValidateParameter<TParam1>(args[0]);
            ValidateParameter<TParam2>(args[1]);
            ValidateParameter<TParam3>(args[2]);
            ValidateParameter<TParam4>(args[3]);
            _method((TParam1)args[0], (TParam2)args[1], (TParam3)args[2], (TParam4)args[3]);
        }
    }
}

