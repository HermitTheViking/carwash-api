using Serilog;

namespace Utility.Logging
{
    public class LogFactory : ILogFactory
    {
        public ILog CreateLog<T>()
        {
            return new SerilogLogger(Log.ForContext<T>());
        }
    }
}