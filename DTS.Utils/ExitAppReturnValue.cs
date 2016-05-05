using DTS.Utils.Core;

namespace DTS.Utils
{
    internal class ExitAppReturnValue : ReturnValue
    {
        public ExitAppReturnValue() 
            : base(ErrorType.None, "")
        {
        }

        public override ReturnValueType ReturnValueType => ReturnValueType.ExitApplication;
    }
}