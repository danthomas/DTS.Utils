namespace DTS.Utils.Core
{
    public interface IOutput
    {
        void WriteLine(string output);
        void WriteReturnValue(ReturnValue returnValue);
        string ReadLine();
    }
}