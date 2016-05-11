using System.IO;
using System.Linq;
using System.Reflection;
using DTS.Utils.Core;
using DTS.Utils.TypeDefs;


namespace MPD.V2.Utils.Cli.BuilderGenerate
{
    class BuilderGeneratorCli : UtilBase
    {
        private readonly string _cliSourceFileDirPath;
        private static Assembly[] _assemblies;

        public BuilderGeneratorCli() 
            :base("gen", "Generate Builders &...")
        {
            Command<Args, CommandType, Context>()
                .Arg("a", a => a.AssemblyFilePath, true)
                .Arg("o", a => a.OutputDirPath, true)
                .NoOp(GenFiles);
        }

        private ReturnValue GenFiles(Args arg1, CommandType arg2, Context arg3)
        {
            return null;

        }

        internal class Context
        {
        }

        internal enum CommandType
        {

        }

        internal class Args
        {
            public string AssemblyFilePath { get; set; }
            public string OutputDirPath { get; set; }
        }

        //public override bool Run()
        //{
        //    _assemblies = new[] { typeof(CommunicationChannel).Assembly,
        //        typeof(SuccessfulDeliveryEmailModel).Assembly,
        //        typeof(Order).Assembly};
        //    
        //    SelectOption("Which Assembly?", _assemblies.Select(x => x.GetName().Name).ToArray(), j =>
        //    {
        //        BuildBuilders(_assemblies[j]);
        //    });
        //
        //    return true;
        //}

        private void BuildBuilders(Assembly assembly)
        {
            string buildersDirectory = Path.Combine(_cliSourceFileDirPath.Replace("\\MPD.V2.Utils.Cli", ""), "MPD.V2.Utils", "Builders");

            var builders = new BuilderGenerator(new TypeBuilder()).GenBuilders(assembly);

            foreach (var genFile in builders)
            {


                if (Path.GetInvalidFileNameChars().Any(x => genFile.FilePath.Replace("\\", "").Contains(x)))
                {
                }
                else
                {
                string filePath = Path.Combine(buildersDirectory, genFile.FilePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.WriteAllText(filePath, genFile.Text);
                }
            }
        }
    }
}