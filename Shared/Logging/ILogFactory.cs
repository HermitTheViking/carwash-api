namespace Utility.Logging
{
    public interface ILogFactory
    {
        ILog CreateLog<T>();
    }
}