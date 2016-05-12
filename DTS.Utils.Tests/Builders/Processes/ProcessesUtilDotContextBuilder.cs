/*Generated using MPD.V2.Utils.Cli*/

// ReSharper disable once CheckNamespace
namespace DTS.Utils.Processes.Builders
{
    public class ProcessesUtilDotContextBuilder
    {
        private IProcess[] _processes;

        public static IProcess[] DefaultProcesses { get; set; }

        static ProcessesUtilDotContextBuilder()
        {
        }

        private ProcessesUtilDotContextBuilder()
        {
            _processes = DefaultProcesses;
        }

        public static ProcessesUtil.Context Default()
        {
            return New.Build();
        }

        public static ProcessesUtilDotContextBuilder New
        {
            get { return new ProcessesUtilDotContextBuilder(); }
        }

        public ProcessesUtilDotContextBuilder WithProcesses(IProcess[] processes)
        {
            _processes = processes;
            return this;
        }

        public ProcessesUtil.Context Build()
        {
            return new ProcessesUtil.Context
            {
                Processes = _processes
            };
        }
    }
}