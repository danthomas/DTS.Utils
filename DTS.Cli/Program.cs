using DTS.Utils;
using DTS.Utils.Core;
using DTS.Utils.Generator;

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
                .Util<GeneratorUtil>()
                .Run(inputOutput, inputOutput);
        }
    }
}
