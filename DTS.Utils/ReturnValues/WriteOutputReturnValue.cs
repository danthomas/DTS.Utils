using DTS.Utils.Core;
using DTS.Utils.Details;

namespace DTS.Utils.ReturnValues
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