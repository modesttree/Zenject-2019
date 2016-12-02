using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public static class CommandExtensions
    {
        public static ConditionBinder DeclareCommand<T>(this DiContainer container)
            where T : ICommand
        {
            return container.Bind<T>().AsSingle();
        }

        public static CommandHandlerBinder ImplementCommand<TCommand>(this DiContainer container)
            where TCommand : Command
        {
            var binder = container.StartBinding();
            return new CommandHandlerBinder(
                container, typeof(TCommand), binder);
        }

        public static CommandHandlerBinder<TParam1> ImplementCommand<TParam1, TCommand>(this DiContainer container)
            where TCommand : Command<TParam1>
        {
            var binder = container.StartBinding();
            return new CommandHandlerBinder<TParam1>(
                container, typeof(TCommand), binder);
        }

        public static CommandHandlerBinder<TParam1, TParam2> ImplementCommand<TParam1, TParam2, TCommand>(this DiContainer container)
            where TCommand : Command<TParam1, TParam2>
        {
            var binder = container.StartBinding();
            return new CommandHandlerBinder<TParam1, TParam2>(
                container, typeof(TCommand), binder);
        }

        public static CommandHandlerBinder<TParam1, TParam2, TParam3> ImplementCommand<TParam1, TParam2, TParam3, TCommand>(this DiContainer container)
            where TCommand : Command<TParam1, TParam2, TParam3>
        {
            var binder = container.StartBinding();
            return new CommandHandlerBinder<TParam1, TParam2, TParam3>(
                container, typeof(TCommand), binder);
        }

        public static CommandHandlerBinder<TParam1, TParam2, TParam3, TParam4> ImplementCommand<TParam1, TParam2, TParam3, TParam4, TCommand>(this DiContainer container)
            where TCommand : Command<TParam1, TParam2, TParam3, TParam4>
        {
            var binder = container.StartBinding();
            return new CommandHandlerBinder<TParam1, TParam2, TParam3, TParam4>(
                container, typeof(TCommand), binder);
        }
    }
}
