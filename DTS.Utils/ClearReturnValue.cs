using DTS.Utils.Core;

namespace DTS.Utils
{
    internal class ClearReturnValue : ReturnValue
    {
        public ClearReturnValue() 
            : base(ErrorType.None, "")
        {
        }

        public override ReturnValueType ReturnValueType => ReturnValueType.Clear;
    }
}