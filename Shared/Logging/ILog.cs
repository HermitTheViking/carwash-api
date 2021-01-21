using System;

namespace Utility.Logging
{
    public interface ILog
    {
        ILog ForContext(string propertyName, object value);

        void Debug(string messageTemplate, params object[] propertyValues);
        void Debug(Exception exception, string messageTemplate, params object[] propertyValues);

        void Info(string messageTemplate, params object[] propertyValues);
        void Info(Exception exception, string messageTemplate, params object[] propertyValues);

        void Verbose(string messageTemplate, params object[] propertyValues);
        void Verbose(Exception exception, string messageTemplate, params object[] propertyValues);

        void Warning(string messageTemplate, params object[] propertyValues);
        void Warning(Exception exception, string messageTemplate, params object[] propertyValues);

        void Error(string messageTemplate, params object[] propertyValues);
        void Error(Exception exception, string messageTemplate, params object[] propertyValues);
    }
}