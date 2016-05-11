using System.Reflection;
using DTS.Utils.Core;
using DTS.Utils.Details;
using DTS.Utils.TypeDefs;
using MPD.V2.Utils.Cli.BuilderGenerate;

namespace DTS.Utils.BuilderGenerate
{
    class BuilderGeneratorCli : UtilBase
    {
        private readonly BuilderGenerator _builderGenerator;

        public BuilderGeneratorCli() 
            :base("gen", "Generate Builders &...")
        {
            var typeBuilder = new TypeBuilder();
            _builderGenerator = new BuilderGenerator(typeBuilder);

            Command<Args, CommandType, Context>()
                .Action(CommandType.Bld, "Generate Builders")
                .Action(CommandType.Ver, "Generate Verifiers")
                .Arg("a", a => a.AssemblyFilePath, true)
                .Arg("o", a => a.OutputDirPath, true)
                .WriteFiles(GenFiles);
        }

        private WriteFilesDetails GenFiles(Args args, CommandType commandType, Context context)
        {
            GenFile[] genFiles = null;

            if (commandType == CommandType.Bld)
            {
                genFiles = _builderGenerator.GenBuilders(Assembly.LoadFile(args.AssemblyFilePath)).ToArray();
            }

            return new WriteFilesDetails {DirPath = args.OutputDirPath, GenFiles = genFiles};
        }

        internal class Context
        {
        }

        internal enum CommandType
        {
            Bld,
            Ver
        }

        internal class Args
        {
            public string AssemblyFilePath { get; set; }
            public string OutputDirPath { get; set; }
        }

        //private void BuildBuilders(Assembly assembly)
        //{
        //    string buildersDirectory = Path.Combine(_cliSourceFileDirPath.Replace("\\MPD.V2.Utils.Cli", ""), "MPD.V2.Utils", "Builders");
        //
        //    var builders = new BuilderGenerator(new TypeBuilder()).GenBuilders(assembly);
        //
        //    foreach (var genFile in builders)
        //    {
        //
        //
        //        if (Path.GetInvalidFileNameChars().Any(x => genFile.RelativeFilePath.Replace("\\", "").Contains(x)))
        //        {
        //        }
        //        else
        //        {
        //        string filePath = Path.Combine(buildersDirectory, genFile.RelativeFilePath);
        //            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        //            File.WriteAllText(filePath, genFile.Text);
        //        }
        //    }
        //}
    }
}