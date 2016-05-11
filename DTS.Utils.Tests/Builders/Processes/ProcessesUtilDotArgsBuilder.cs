// ReSharper disable once CheckNamespace
namespace DTS.Utils.Processes.Builders
{
    public class ProcessesUtilDotArgsBuilder
    {
        private string _name;
        private string _filePath;

        public static string DefaultName { get; set; }
        public static string DefaultFilePath { get; set; }

        static ProcessesUtilDotArgsBuilder()
        {
            DefaultName = "";
            DefaultFilePath = "";
        }

        private ProcessesUtilDotArgsBuilder()
        {
            _name = DefaultName;
            _filePath = DefaultFilePath;
        }

        public static ProcessesUtil.Args Default()
        {
            return New.Build();
        }

        public static ProcessesUtilDotArgsBuilder New
        {
            get { return new ProcessesUtilDotArgsBuilder(); }
        }

        public ProcessesUtilDotArgsBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ProcessesUtilDotArgsBuilder WithFilePath(string filePath)
        {
            _filePath = filePath;
            return this;
        }

        public ProcessesUtil.Args Build()
        {
            return new ProcessesUtil.Args
            {
                Name = _name,
                FilePath = _filePath,
            };

        }
    }
}