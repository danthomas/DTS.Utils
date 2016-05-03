namespace DTS.Utils.Core
{
    public interface ICommand
    {
        string Name { get; set; }

        ReturnValue Execute(string[] args);
    }
}