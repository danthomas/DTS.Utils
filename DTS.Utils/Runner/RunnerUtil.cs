using System;
using System.IO;
using DTS.Utils.Core;

namespace DTS.Utils.Runner
{
    public class RunnerUtil : UtilBase
    {
        public RunnerUtil() : base("root", "")
        {
            Command<EmptyArgs, CommandType, Context>()
              .Action(CommandType.Cls, "Clears the console")
              .NoOp(Clear);

            Command<EmptyArgs, CommandType, Context>()
              .Action(CommandType.Exit, "Exit the application")
              .NoOp(Exit);

            Command<CurrArgs, CommandType, Context>()
              .Action(CommandType.Curr, "Sets the current working directory")
              .Arg("p", x => x.DirectoryPath, true)
              .NoOp(SetCurrentWorkingDirectory);
        }

        private ReturnValue Clear(EmptyArgs arg1, CommandType arg2, Context context)
        {
            return ReturnValue.Ok(ReturnValueType.Clear);
        }

        private ReturnValue Exit(EmptyArgs arg1, CommandType arg2, Context context)
        {
            return ReturnValue.Ok(ReturnValueType.ExitApplication);
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

        public class Context
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


        internal class Args
        {
        }
    }
}
