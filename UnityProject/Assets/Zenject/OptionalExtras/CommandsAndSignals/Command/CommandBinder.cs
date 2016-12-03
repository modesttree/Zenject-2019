using System;
using System.Collections.Generic;

namespace Zenject
{
    public class CommandBinder : ConditionBinder
    {
        readonly CommandSettings _commandSettings;

        public CommandBinder(
            BindInfo bindInfo, CommandSettings commandSettings)
            : base(bindInfo)
        {
            _commandSettings = commandSettings;
        }

        public ConditionBinder RequireHandler()
        {
            _commandSettings.MinHandlers = 1;
            return this;
        }

        public ConditionBinder RequireSingleHandler()
        {
            _commandSettings.MinHandlers = 1;
            _commandSettings.MaxHandlers = 1;
            return this;
        }
    }
}
