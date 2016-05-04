namespace DTS.Utils.Core
{
    public interface ICommand
    {
        string[] Names { get; set; }

        ReturnValue Execute(string[] args);
    }
}