using DTS.Utils.Core;

namespace DTS.Utils.Details
{
    public class IfDetails
    {
        public bool If { get; set; }
        public string Message { get; set; }
    }

    public class IfThenDetails
    {
        public bool If { get; set; }
        public ICommand Command { get; set; }
    }
}