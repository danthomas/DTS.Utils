using DTS.Utils;
using DTS.Utils.Core;

namespace DTS.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputOutput = new InputOutput();

            var processRunner = new ProcessRunner();

            new UtilRunner(processRunner)
                .Util<Utils.WindowsServices.WindowsServiceUtil>()
                .Util<Utils.Processes.ProcessesUtil>()
                .Util<Utils.Nuget.NugetUtil>()
                .Util<Utils.BuilderGenerate.GeneratorUtil>()
                .Run(inputOutput, inputOutput);
        }
    }
}
