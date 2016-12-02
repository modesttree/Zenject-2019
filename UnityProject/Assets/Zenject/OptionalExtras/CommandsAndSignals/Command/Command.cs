using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    public interface ICommand
    {
    }

    public abstract class CommandBase : ICommand
    {
        CommandManager _manager;

        [Inject]
        void Construct(CommandManager manager)
        {
            _manager = manager;
        }

        protected CommandManager Manager
        {
            get { return _manager; }
        }

        public bool HasHandler
        {
            get
            {
                return _manager.IsHandlerRegistered(this.GetType());
            }
        }
    }

    public abstract class Command : CommandBase
    {
        public void Execute()
        {
            Manager.Trigger(this.GetType(), new object[0]);
        }
    }

    public abstract class Command<TParam1> : CommandBase
    {
        public void Execute(TParam1 param1)
        {
            Manager.Trigger(this.GetType(), new object[] { param1 });
        }
    }

    public abstract class Command<TParam1, TParam2> : CommandBase
    {
        public void Execute(TParam1 param1, TParam2 param2)
        {
            Manager.Trigger(this.GetType(), new object[] { param1, param2 });
        }
    }

    public abstract class Command<TParam1, TParam2, TParam3> : CommandBase
    {
        public void Execute(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            Manager.Trigger(this.GetType(), new object[] { param1, param2, param3 });
        }
    }

    public abstract class Command<TParam1, TParam2, TParam3, TParam4> : CommandBase
    {
        public void Execute(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            Manager.Trigger(this.GetType(), new object[] { param1, param2, param3, param4 });
        }
    }
}
