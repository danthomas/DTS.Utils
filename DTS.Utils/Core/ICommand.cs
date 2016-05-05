namespace DTS.Utils.Core
{
    public interface ICommand
    {
        string[] Names { get; set; }
        string ArgsDescription { get; }
        ReturnValue Execute(string[] args);
    }
}