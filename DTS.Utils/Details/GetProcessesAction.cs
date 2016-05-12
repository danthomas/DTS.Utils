using System;
using DTS.Utils.Processes;

namespace DTS.Utils.Details
{
    public class GetProcessesAction
    {
        public Action<IProcess[]> Action { get; set; }
    }
}