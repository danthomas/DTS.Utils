using System;

namespace DTS.Utils
{
    public class SelectOptionDetails
    {
        public string Message { get; set; }
        public string[] Options { get; set; }
        public Action<string> OptionSelected { get; set; }
    }
}