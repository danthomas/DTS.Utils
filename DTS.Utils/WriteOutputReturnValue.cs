using DTS.Utils.Core;

namespace DTS.Utils
{
    public class WriteOutputReturnValue : ReturnValue
    {
        public string[] Lines { get; set; }
        public RunProcessDetails RunProcessDetails { get; set; }
        
        public WriteOutputReturnValue(string[] lines)
            : base(ErrorType.None, "")
        {
            Lines = lines;
        }
    }
}