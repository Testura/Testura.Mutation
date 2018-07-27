using System;
using log4net;
using log4net.Appender;

namespace Cama.Core.Logs
{
    public class LogWatcher
    {
        private readonly MemoryAppenderWithEvents memoryAppender;

        public LogWatcher()
        {
            var reps = LogManager.GetRepository().GetAppenders();

            // Get the memory appender
            memoryAppender = (MemoryAppenderWithEvents)Array.Find(LogManager.GetRepository().GetAppenders(), GetMemoryAppender);

            // Add an event handler to handle updates from the MemoryAppender
            memoryAppender.Updated += HandleUpdate;
        }

        public event EventHandler<string> NewMessage;

        public string LogContent { get; private set; }

        public void HandleUpdate(object sender, log4net.Core.LoggingEvent logEvent)
        {
            NewMessage?.Invoke(this, $"{logEvent.TimeStamp:yyyy-MM-dd HH:mm:ss,fff}[{logEvent.Level}]: {logEvent.RenderedMessage} \r\n");
        }

        private static bool GetMemoryAppender(IAppender appender)
        {
            // Returns the IAppender named MemoryAppender in the Log4Net.config file
            if (appender.Name.Equals("MemoryAppender"))
            {
                return true;
            }

            return false;
        }
    }
}
