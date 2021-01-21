using Serilog;
using System;

namespace Utility.Logging
{
    public class SerilogLogger : ILog
    {
        private readonly ILogger _log;

        public SerilogLogger(ILogger log)
        {
            _log = log;
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            _log.Debug(messageTemplate, propertyValues);
        }

        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Debug(exception, messageTemplate, propertyValues);
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            _log.Error(messageTemplate, propertyValues);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Error(exception, messageTemplate, propertyValues);
        }

        public void Info(string messageTemplate, params object[] propertyValues)
        {
            _log.Information(messageTemplate, propertyValues);
        }

        public void Info(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Information(exception, messageTemplate, propertyValues);
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            _log.Verbose(messageTemplate, propertyValues);
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Verbose(exception, messageTemplate, propertyValues);
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            _log.Warning(messageTemplate, propertyValues);
        }

        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _log.Warning(exception, messageTemplate, propertyValues);
        }

        public ILog ForContext(string propertyName, object value)
        {
            return new SerilogLogger(_log.ForContext(propertyName, value));
        }
    }
}