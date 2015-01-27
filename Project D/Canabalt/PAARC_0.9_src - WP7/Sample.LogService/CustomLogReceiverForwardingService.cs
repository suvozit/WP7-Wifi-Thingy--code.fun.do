using System;
using System.Linq;
using NLog;
using NLog.LogReceiverService;

namespace Sample.LogService
{
    internal class CustomLogReceiverForwardingService : ILogReceiverServer
    {
        public event EventHandler<LogEventArgs> LogEventReceived;

        #region Implementation of ILogReceiverServer

        public void ProcessLogMessages(NLogEvents events)
        {
            // Peter Kuhn: all this is basically taken from the original code,
            // but I've fixed the erroneous parsing of the message parameters
            // in the places marked with "fixed" comments below.
            // Details about the problem can be found here:
            // http://www.pitorque.de/MisterGoodcat/post/Fixing-NLogs-Logging-to-a-Web-Service.aspx

            var baseTimeUtc = new DateTime(events.BaseTimeUtc, DateTimeKind.Utc);
            var logEvents = new LogEventInfo[events.Events.Length];

            // convert transport representation of log events into workable LogEventInfo[]
            for (int j = 0; j < events.Events.Length; ++j)
            {
                var ev = events.Events[j];
                LogLevel level = LogLevel.FromOrdinal(ev.LevelOrdinal);
                string loggerName = events.Strings[ev.LoggerOrdinal];

                var logEventInfo = new LogEventInfo();
                logEventInfo.Level = level;
                logEventInfo.LoggerName = loggerName;
                logEventInfo.TimeStamp = baseTimeUtc.AddTicks(ev.TimeDelta);
                logEventInfo.Message = events.Strings[ev.MessageOrdinal];
                logEventInfo.Properties.Add("ClientName", events.ClientName);

                // fixed: ev.Values does not contain the actual values, but the indices of the entries
                // in the Strings property, separated by pipe characters
                var values = ev.Values
                    .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();

                for (int i = 0; i < events.LayoutNames.Count; ++i)
                {
                    // fixed: use the correct strings
                    //logEventInfo.Properties.Add(events.LayoutNames[i], ev.Values[i]);
                    var index = values[i];
                    logEventInfo.Properties.Add(events.LayoutNames[i], events.Strings[index]);
                }

                logEvents[j] = logEventInfo;
            }

            this.ProcessLogMessages(logEvents);
        }

        #endregion

        protected virtual void ProcessLogMessages(LogEventInfo[] logEvents)
        {
            Logger logger = null;
            string lastLoggerName = string.Empty;

            foreach (var ev in logEvents)
            {
                if (ev.LoggerName != lastLoggerName)
                {
                    logger = LogManager.GetLogger(ev.LoggerName);
                    lastLoggerName = ev.LoggerName;
                }

                // both log ourselves and raise event
                logger.Log(ev);
                RaiseLogEventReceivedEvent(ev);
            }
        }

        private void RaiseLogEventReceivedEvent(LogEventInfo logEvent)
        {
            var handlers = LogEventReceived;
            if (handlers != null)
            {
                handlers(this, new LogEventArgs(logEvent));
            }
        }
    }
}
