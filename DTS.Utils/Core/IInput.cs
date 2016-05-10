namespace DTS.Utils.Core
{
    public interface IInput
    {
        T ReadLine<T>();
    }
}