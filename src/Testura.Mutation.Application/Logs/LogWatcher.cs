using System;
using System.Linq;
using log4net;

namespace Testura.Mutation.Application.Logs
{
    public class LogWatcher
    {
        private readonly MemoryAppenderWithEvents memoryAppender;

        public LogWatcher()
        {
            var appenders = LogManager.GetRepository().GetAppenders();
            memoryAppender = appenders.FirstOrDefault(a => a.Name.Equals("MemoryAppender")) as MemoryAppenderWithEvents;

            if (memoryAppender == null)
            {
                throw new Exception($"Could not find any memory appender. We currently have these appenders: {string.Join(", ", appenders.Select(a => a.Name))}");
            }

            // Add an event handler to handle updates from the MemoryAppender
            memoryAppender.Updated += HandleUpdate;
        }

        public event EventHandler<string> NewMessage;

        public string LogContent { get; private set; }

        public void HandleUpdate(object sender, log4net.Core.LoggingEvent logEvent)
        {
            NewMessage?.Invoke(this, $"{logEvent.TimeStamp:yyyy-MM-dd HH:mm:ss,fff}[{logEvent.Level}]: {logEvent.RenderedMessage} \r\n");
        }
    }
}
