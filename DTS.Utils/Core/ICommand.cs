namespace DTS.Utils.Core
{
    public interface ICommand
    {
        Act[] Acts { get; set; }
        string ArgsDescription { get; }
        ReturnValue Init(string[] args);
        ReturnValue ExecuteFunc();
    }
}