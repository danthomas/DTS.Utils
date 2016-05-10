namespace DTS.Utils.Core
{
    public interface IOutput
    {
        void WriteReturnValue(ReturnValue returnValue);
        void Clear();
        void WriteLines(params string[] lines);
    }
}