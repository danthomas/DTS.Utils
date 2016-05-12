using System;

namespace DTS.Utils.Details
{
    public class RunProcessAction
    {
        public RunProcessDetails RunProcessDetails { get; set; }
        public Action<string> Action { get; set; }
    }
}