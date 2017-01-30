using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public class CommandManager : ILateDisposable
    {
        readonly Dictionary<Type, List<ICommandHandler>> _commandHandlers = new Dictionary<Type, List<ICommandHandler>>();

        public bool IsHandlerRegistered(Type commandType)
        {
            return !GetList(commandType).IsEmpty();
        }

        List<ICommandHandler> GetList(Type commandType)
        {
            List<ICommandHandler> handlers;

            if (!_commandHandlers.TryGetValue(commandType, out handlers))
            {
                handlers = new List<ICommandHandler>();
                _commandHandlers.Add(commandType, handlers);
            }

            return handlers;
        }

        public void Register(Type commandType, ICommandHandler handler)
        {
            GetList(commandType).Add(handler);
        }

        public void Unregister(Type commandType, ICommandHandler handler)
        {
            GetList(commandType).RemoveWithConfirm(handler);
        }

        public void LateDispose()
        {
            Assert.Warn(_commandHandlers.Values.SelectMany(x => x).IsEmpty(),
                "Found commands still registered on CommandManager");
        }

        public void Trigger(
            Type commandType, CommandSettings settings, object[] args)
        {
            var handlers = GetList(commandType);

            if (handlers.Count < settings.MinHandlers)
            {
                if (settings.MinHandlers == 0)
                {
                    throw Assert.CreateException(
                        "Fired command '{0}' but no handler was registered!  Command requires at least one handler.", commandType);
                }

                throw Assert.CreateException(
                    "Command '{0}' was fired but only had {1} handlers attached (requires at least {2}).",
                    commandType, handlers.Count, settings.MinHandlers);
            }

            if (handlers.Count > settings.MaxHandlers)
            {
                if (settings.MaxHandlers == 1)
                {
                    throw Assert.CreateException(
                        "Found multiple handlers for command '{0}'.  Was expecting only one.", commandType);
                }

                throw Assert.CreateException(
                    "Found '{0}' handlers for command '{1}' but was expecting a max of '{2}'",
                    handlers.Count, commandType, settings.MaxHandlers);
            }

            foreach (var handler in handlers)
            {
                handler.Execute(args);
            }
        }
    }
}

