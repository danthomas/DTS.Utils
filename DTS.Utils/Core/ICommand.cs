namespace DTS.Utils.Core
{
    public interface ICommand
    {
        string[] Names { get; set; }
        string ArgsDescription { get; }
        ReturnValue Init(string[] args);
        ReturnValue ExecuteFunc();
    }
}