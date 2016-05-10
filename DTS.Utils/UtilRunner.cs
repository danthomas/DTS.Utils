using System.Collections.Generic;
using System.Linq;
using DTS.Utils.Core;
using DTS.Utils.Runner;

namespace DTS.Utils
{
    public class UtilRunner
    {
        private readonly IProcessRunner _processRunner;
        private readonly List<UtilBase> _utils;
        private readonly RunnerUtil _runnerUtil;
        private UtilBase _currentUtil;

        public UtilRunner(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
            _utils = new List<UtilBase>();
            _currentUtil = _runnerUtil = new RunnerUtil();
        }

        public UtilRunner Util<T>() where T : UtilBase, new()
        {
            T t = new T();
            _utils.Add(t);
            return this;
        }

        public void Run(IInput input, IOutput output)
        {
            while (true)
            {
                var line = input.ReadLine();

                if (TrySetCurrentUtil(output, line))
                    continue;

                var getCommandReturnValue = GetCommand(line);

                if (getCommandReturnValue.IsSuccess)
                {
                    CommandDetails data = getCommandReturnValue.Data;

                    data.Command.Init(data.Args);

                    ReturnValue executeReturnValue;

                    bool exitApplication = false;

                    do
                    {
                        executeReturnValue = data.Command.ExecuteFunc();

                        if (executeReturnValue.ReturnValueType == ReturnValueType.ExitApplication)
                        {
                            exitApplication = true;
                            break;
                        }

                        if (executeReturnValue.ReturnValueType == ReturnValueType.Clear)
                        {
                            output.Clear();
                            break;
                        }

                        RunProcessReturnValue runProcessReturnValue = executeReturnValue as RunProcessReturnValue;
                        WriteOutputReturnValue writeOutputReturnValue = executeReturnValue as WriteOutputReturnValue;

                        if (runProcessReturnValue != null)
                        {
                            executeReturnValue = _processRunner.Run(runProcessReturnValue.RunProcessDetails);

                            if (executeReturnValue.IsSuccess)
                            {
                                runProcessReturnValue.RunProcessDetails.SetOutput(executeReturnValue.Message);
                            }
                        }
                        else if (writeOutputReturnValue != null)
                        {
                            output.WriteLines(writeOutputReturnValue.Lines);
                        }
                        else
                        {
                            output.WriteReturnValue(executeReturnValue);
                        }

                    } while (executeReturnValue.IsSuccess);

                    if (exitApplication)
                    {
                        break;
                    }
                }
                else
                {
                    output.WriteLines($"Util or Command {line} not recognised");
                }
            }
        }

        private bool TrySetCurrentUtil(IOutput output, string line)
        {
            var util = GetUtil(line);

            if (util != null)
            {
                _currentUtil = util;
                output.WriteLines($"--->{util.Name}: {util.Description}");
                return true;
            }
            return false;
        }

        private ReturnValue<CommandDetails> GetCommand(string line)
        {
            var returnValue = _currentUtil.GetCommand(line);

            if (!returnValue.IsSuccess)
            {
                returnValue = _runnerUtil.GetCommand(line);
            }

            return returnValue;
        }

        private UtilBase GetUtil(string line)
        {
            return _utils.FirstOrDefault(x => x.Name == line);
        }
    }
}