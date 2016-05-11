using System;

namespace DTS.Utils.Details
{
    public class SelectOptionAction

    {
        public string Message { get; set; }
        public string[] Options { get; set; }
        public Action<string> OptionSelected { get; set; }
    }
}