using System;
using System.Collections.Generic;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    public class StaticMethodSignalHandler : SignalHandlerBase
    {
        readonly Action _method;

        public StaticMethodSignalHandler(
            BindingId signalId, SignalManager manager, Action method)
            : base(signalId, manager)
        {
            _method = method;
        }

        public override void Execute(object[] args)
        {
            Assert.That(args.IsEmpty());

#if UNITY_EDITOR
            using (ProfileBlock.Start(_method.ToDebugString()))
#endif
            {
                _method();
            }
        }
    }

    public class StaticMethodSignalHandler<TParam1> : SignalHandlerBase
#if ENABLE_IL2CPP
        // See discussion here for why we do this: https://github.com/modesttree/Zenject/issues/219#issuecomment-284751679
        where TParam1 : class
#endif
    {
        readonly Action<TParam1> _method;

        public StaticMethodSignalHandler(
            BindingId signalId, SignalManager manager, Action<TParam1> method)
            : base(signalId, manager)
        {
            _method = method;
        }

        public override void Execute(object[] args)
        {
            Assert.That(args.IsLength(1));
            ValidateParameter<TParam1>(args[0]);
#if UNITY_EDITOR
            using (ProfileBlock.Start(_method.ToDebugString()))
#endif
            {
                _method((TParam1)args[0]);
            }
        }
    }

    public class StaticMethodSignalHandler<TParam1, TParam2> : SignalHandlerBase
#if ENABLE_IL2CPP
        // See discussion here for why we do this: https://github.com/modesttree/Zenject/issues/219#issuecomment-284751679
        where TParam1 : class
        where TParam2 : class
#endif
    {
        readonly Action<TParam1, TParam2> _method;

        public StaticMethodSignalHandler(
            BindingId signalId, SignalManager manager, Action<TParam1, TParam2> method)
            : base(signalId, manager)
        {
            _method = method;
        }

        public override void Execute(object[] args)
        {
            Assert.That(args.IsLength(1));
            ValidateParameter<TParam1>(args[0]);
            ValidateParameter<TParam2>(args[1]);
#if UNITY_EDITOR
            using (ProfileBlock.Start(_method.ToDebugString()))
#endif
            {
                _method((TParam1)args[0], (TParam2)args[1]);
            }
        }
    }

    public class StaticMethodSignalHandler<TParam1, TParam2, TParam3> : SignalHandlerBase
#if ENABLE_IL2CPP
        // See discussion here for why we do this: https://github.com/modesttree/Zenject/issues/219#issuecomment-284751679
        where TParam1 : class
        where TParam2 : class
        where TParam3 : class
#endif
    {
        readonly Action<TParam1, TParam2, TParam3> _method;

        public StaticMethodSignalHandler(
            BindingId signalId, SignalManager manager, Action<TParam1, TParam2, TParam3> method)
            : base(signalId, manager)
        {
            _method = method;
        }

        public override void Execute(object[] args)
        {
            Assert.That(args.IsLength(1));
            ValidateParameter<TParam1>(args[0]);
            ValidateParameter<TParam2>(args[1]);
            ValidateParameter<TParam3>(args[2]);
#if UNITY_EDITOR
            using (ProfileBlock.Start(_method.ToDebugString()))
#endif
            {
                _method((TParam1)args[0], (TParam2)args[1], (TParam3)args[2]);
            }
        }
    }

    public class StaticMethodSignalHandler<TParam1, TParam2, TParam3, TParam4> : SignalHandlerBase
#if ENABLE_IL2CPP
        // See discussion here for why we do this: https://github.com/modesttree/Zenject/issues/219#issuecomment-284751679
        where TParam1 : class
        where TParam2 : class
        where TParam3 : class
        where TParam4 : class
#endif
    {
        readonly Action<TParam1, TParam2, TParam3, TParam4> _method;

        public StaticMethodSignalHandler(
            BindingId signalId, SignalManager manager, Action<TParam1, TParam2, TParam3, TParam4> method)
            : base(signalId, manager)
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
#if UNITY_EDITOR
            using (ProfileBlock.Start(_method.ToDebugString()))
#endif
            {
                _method((TParam1)args[0], (TParam2)args[1], (TParam3)args[2], (TParam4)args[3]);
            }
        }
    }
}

