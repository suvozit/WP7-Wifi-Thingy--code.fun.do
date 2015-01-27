using System;
using NLog;

namespace Sample.LogService
{
    internal class LogEventArgs : EventArgs
    {
        public LogEventInfo LogEvent
        {
            get;
            private set;
        }

        public LogEventArgs(LogEventInfo logEvent)
        {
            LogEvent = logEvent;
        }
    }
}