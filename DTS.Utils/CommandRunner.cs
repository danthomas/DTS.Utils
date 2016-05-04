using System;
using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;

namespace DTS.Utils
{
    public class CommandRunner
    {
        private readonly IRunner _runner;
        private readonly List<ICommand> _commands;

        public CommandRunner(IRunner runner)
        {
            _runner = runner;
            _commands = new List<ICommand>();
        }

        protected Command<T, A> Command<T, A>() where T : class, new()
        {
            Command<T, A> command = new Command<T, A>(this);

            _commands.Add(command);

            return command;
        }

        public ReturnValue Execute(string line)
        {
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var command = _commands.SingleOrDefault(x => x.Names.Contains(parts[0].ToLower()));

            if (command == null)
            {
                return ReturnValue.Error(String.Format(@"Command {0} not recognised", parts[0]));
            }

            return command.Execute(parts);
        }

        public ReturnValue Run(RunDetails runDetails)
        {
            return _runner.Run(runDetails);
        }
    }
}