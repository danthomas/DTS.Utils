using DTS.Utils;

namespace DTS.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputOutput = new InputOutput();

            var processRunner = new ProcessRunner();

            new UtilRunner()
                .Util(new Utils.WindowsServices.Util(processRunner))
                .Run(inputOutput, inputOutput);
        }
    }
}
