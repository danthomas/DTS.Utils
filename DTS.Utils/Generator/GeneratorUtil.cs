using System.Reflection;
using DTS.Utils.Core;
using DTS.Utils.Details;
using DTS.Utils.TypeDefs;

namespace DTS.Utils.Generator
{
    public class GeneratorUtil : UtilBase
    {
        private readonly BuilderGenerator _builderGenerator;

        public GeneratorUtil() 
            :base("gen", "Generate Builders &...")
        {
            var typeBuilder = new TypeBuilder();
            _builderGenerator = new BuilderGenerator(typeBuilder);

            Command<Args, CommandType, Context>()
                .Action(CommandType.Bld, "Generate Builders")
                .Action(CommandType.Ver, "Generate Verifiers")
                .Arg("a", a => a.AssemblyFilePath, true)
                .Arg("o", a => a.OutputDirPath, true)
                .Arg("c", a => a.ClearOutputDir)
                .LoadAssembly(c => c.Args.AssemblyFilePath, (a, c) => c.Assembly = a)
                .IfThen(ClearDirectory, command => command.NoOp(Tester))
                .WriteFiles(GenFiles);
        }

        private ReturnValue Tester(Context arg)
        {
            return ReturnValue.Ok("Tester");
        }

        private bool ClearDirectory(Context context)
        {
            return context.Args.ClearOutputDir;
        }

        private WriteFilesAction GenFiles(Context context)
        {
            GenFile[] genFiles = null;

            if (context.Args.ClearOutputDir)
            {
            }

            if (context.CommandType == CommandType.Bld)
            {
                genFiles = _builderGenerator.GenBuilders(context.Assembly).ToArray();
            }

            return new WriteFilesAction {DirPath = context.Args.OutputDirPath, GenFiles = genFiles};
        }

        internal class Context : Context<Args, CommandType>
        {
            public Assembly Assembly { get; set; }
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
            public bool ClearOutputDir { get; set; }
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