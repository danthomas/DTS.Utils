/*Generated using MPD.V2.Utils.Cli*/

// ReSharper disable once CheckNamespace
namespace DTS.Utils.Processes.Builders
{
    public class ProcessesUtilDotContextBuilder
    {
        private ProcessesUtil.IProcess[] _processes;
        private bool _stopConfirmed;

        public static ProcessesUtil.IProcess[] DefaultProcesses { get; set; }
        public static bool DefaultStopConfirmed { get; set; }

        static ProcessesUtilDotContextBuilder()
        {
            DefaultStopConfirmed = false;
        }

        private ProcessesUtilDotContextBuilder()
        {
            _processes = DefaultProcesses;
            _stopConfirmed = DefaultStopConfirmed;
        }

        public static ProcessesUtil.Context Default()
        {
            return New.Build();
        }

        public static ProcessesUtilDotContextBuilder New
        {
            get { return new ProcessesUtilDotContextBuilder(); }
        }

        public ProcessesUtilDotContextBuilder WithProcesses(ProcessesUtil.IProcess[] processes)
        {
            _processes = processes;
            return this;
        }

        public ProcessesUtilDotContextBuilder WithStopConfirmed(bool stopConfirmed)
        {
            _stopConfirmed = stopConfirmed;
            return this;
        }

        public ProcessesUtil.Context Build()
        {
            return new ProcessesUtil.Context
            {
                Processes = _processes,
                StopConfirmed = _stopConfirmed,
            };
        }
    }
}