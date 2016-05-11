using System;
using DTS.Utils.Core;

namespace DTS.Utils.Details
{
    public class SelectOptionDetails
    {
        public string Message { get; set; }
        public string[] Options { get; set; }
        public Action<string> OptionSelected { get; set; }
    }
    public class WriteFilesDetails
    {
        public string DirPath { get; set; }
        public GenFile[] GenFiles { get; set; }
    }
}