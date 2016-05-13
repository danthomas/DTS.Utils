using System;
using System.IO;
using DTS.Utils.Core;

namespace DTS.Utils.Runner
{
    public class RunnerUtil : UtilBase
    {
        public RunnerUtil() : base("root", "")
        {
            Command<EmptyArgs, CommandType, EmptyContext<CommandType>>()
              .Action(CommandType.Cls, "Clears the console")
              .NoOp(Clear);

            Command<EmptyArgs, CommandType, EmptyContext<CommandType>>()
              .Action(CommandType.Exit, "Exit the application")
              .NoOp(Exit);

            Command<CurrArgs, CommandType, Context>()
              .Action(CommandType.Curr, "Sets the current working directory")
              .Arg("p", x => x.DirectoryPath, true)
              .NoOp(SetCurrentWorkingDirectory);
        }

        private ReturnValue Clear(EmptyContext<CommandType> context)
        {
            return ReturnValue.Ok(ReturnValueType.Clear);
        }

        private ReturnValue Exit(EmptyContext<CommandType> context)
        {
            return ReturnValue.Ok(ReturnValueType.ExitApplication);
        }

        private ReturnValue SetCurrentWorkingDirectory(Context context)
        {
            try
            {
                Directory.SetCurrentDirectory(context.Args.DirectoryPath);

                return ReturnValue.Ok($"Current Directory set to {Directory.GetCurrentDirectory()}");
            }
            catch (Exception e)
            {
                return ReturnValue.Error(ErrorType.FailedToSetCurrentDirectory, $@"Failed to set Current Directory {e.Message}");
            }
        }

        private class Context : Context<CurrArgs, CommandType>
        {
        }

        internal enum CommandType
        {
            Exit,
            Help,
            Curr,
            Cls
        }

        public class CurrArgs
        {
            public string DirectoryPath { get; set; }
        }
    }
}
