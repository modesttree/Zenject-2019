using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public abstract class CommandHandlerBase : ICommandHandler, IDisposable
    {
        readonly CommandManager _manager;
        readonly Type _commandType;

        public CommandHandlerBase(
            Type commandType, CommandManager manager)
        {
            _manager = manager;
            _commandType = commandType;

            manager.Register(commandType, this);
        }

        public void Dispose()
        {
            _manager.Unregister(_commandType);
        }

        protected void ValidateParameter<T>(object value)
        {
            if (value == null)
            {
                Assert.That(!typeof(T).IsValueType);
            }
            else
            {
                Assert.That(value.GetType().DerivesFromOrEqual<T>());
            }
        }

        public abstract void Execute(object[] args);
    }
}


