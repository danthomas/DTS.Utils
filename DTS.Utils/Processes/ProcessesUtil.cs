using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTS.Utils.Processes
{
    public class ProcessesUtil : UtilBase
    {
        public ProcessesUtil() : base("proc", "Processes Utility")
        {
            Command<Args, CommandType, Context>()
                .Action(CommandType.List, "Lists the running processes")
                .WriteOutput(ListProcesses);
        }

        private IEnumerable<string> ListProcesses(Args arg1, CommandType arg2, Context arg3)
        {
            return Process.GetProcesses().Select(x => x.ProcessName);
        }

        enum CommandType
        {
            List
        }
        class Args
        { }
        class Context
        { }
    }
}
