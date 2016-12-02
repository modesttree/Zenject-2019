using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public class CommandManager : ILateDisposable
    {
        readonly Dictionary<Type, ICommandHandler> _commandHandlers = new Dictionary<Type, ICommandHandler>();

        public bool IsHandlerRegistered(Type commandType)
        {
            return _commandHandlers.ContainsKey(commandType);
        }

        public void Register(Type commandType, ICommandHandler handler)
        {
            _commandHandlers.Add(commandType, handler);
        }

        public void Unregister(Type commandType)
        {
            _commandHandlers.RemoveWithConfirm(commandType);
        }

        public void LateDispose()
        {
            Assert.That(_commandHandlers.IsEmpty());
        }

        public void Trigger(Type commandType, object[] args)
        {
            ICommandHandler handler;

            if (!_commandHandlers.TryGetValue(commandType, out handler))
            {
                Log.Warn("Fired command '{0}' but no handler was registered!", commandType);
                return;
            }

            handler.Execute(args);
        }
    }
}

