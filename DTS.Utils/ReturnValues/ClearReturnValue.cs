using DTS.Utils.Core;

namespace DTS.Utils.ReturnValues
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