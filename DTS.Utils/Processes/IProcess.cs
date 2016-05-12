namespace DTS.Utils.Processes
{
    public interface IProcess
    {
        void Stop();
        string Name { get; }
    }
}