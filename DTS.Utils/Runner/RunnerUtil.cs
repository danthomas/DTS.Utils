using System;
using System.Collections.Generic;
using System.IO;
using DTS.Utils.Core;

namespace DTS.Utils.Runner
{
    public class RunnerUtil : UtilBase
    {
        public RunnerUtil() : base("", "")
        {
            Command<EmptyArgs, CommandType, Context>()
              .Action(CommandType.Help, "Details the available utils")
              .NoOp(ShowHelp);

            Command<EmptyArgs, CommandType, Context>()
              .Action(CommandType.Exit, "Exit the application")
              .NoOp(Exit);

            Command<CurrArgs, CommandType, Context>()
              .Action(CommandType.Curr, "Sets the current working directory")
              .Arg("p", x => x.DirectoryPath, true)
              .NoOp(SetCurrentWorkingDirectory);

        }

        public class Context
        {
        }

        private ReturnValue ShowHelp(EmptyArgs args, CommandType commandType, Context context)
        {
            List<string> lines = new List<string>(new[] { $"{Name} commands:" });

            //foreach (ICommand command in _commands)
            //{
            //
            //    foreach (var name in command.Names)
            //    {
            //        lines.Add($"{name}: {command.ArgsDescription}");
            //    }
            //}

            return ReturnValue.Ok(String.Join(Environment.NewLine, lines));
        }

        private ReturnValue Exit(EmptyArgs arg1, CommandType arg2, Context context)
        {
            return new ExitAppReturnValue();
        }

        private ReturnValue SetCurrentWorkingDirectory(CurrArgs args, CommandType commandType, Context context)
        {
            try
            {
                Directory.SetCurrentDirectory(args.DirectoryPath);

                return ReturnValue.Ok($"Current Directory set to {Directory.GetCurrentDirectory()}");
            }
            catch (Exception e)
            {
                return ReturnValue.Error(ErrorType.FailedToSetCurrentDirectory, $@"Failed to set Current Directory {e.Message}");
            }
        }


        internal enum CommandType
        {
            Exit,
            Help,
            Curr
        }

        public class CurrArgs
        {
            public string DirectoryPath { get; set; }
        }


        internal class Args
        {
        }
    }
}
