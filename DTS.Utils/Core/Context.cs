namespace DTS.Utils.Core
{
    public class Context<TA, TC>
        where TA : class, new()
    {
        public TA Args { get; set; }
        public TC CommandType { get; set; }
    }
}