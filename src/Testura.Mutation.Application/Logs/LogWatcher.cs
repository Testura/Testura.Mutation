using System;
using System.Linq;
using log4net;
using log4net.Repository.Hierarchy;

namespace Testura.Mutation.Application.Logs
{
    public class LogWatcher
    {
        private readonly MemoryAppenderWithEvents memoryAppender;

        public LogWatcher()
        {
            var appenders = LogManager.GetRepository().GetAppenders();
            memoryAppender = appenders.FirstOrDefault(a => a.Name.Equals("MemoryAppender")) as MemoryAppenderWithEvents;

            /* Sometimes this turn to null (no idea why). So we create this as a safety check) */
            if (memoryAppender == null)
            {
                memoryAppender = new MemoryAppenderWithEvents();
                var hierarchy = (Hierarchy)LogManager.GetRepository();
                hierarchy.Root.AddAppender(memoryAppender);
            }

            // Add an event handler to handle updates from the MemoryAppender
            memoryAppender.Updated += HandleUpdate;
        }

        public event EventHandler<string> NewMessage;

        public void HandleUpdate(object sender, log4net.Core.LoggingEvent logEvent)
        {
            NewMessage?.Invoke(this, $"{logEvent.TimeStamp:yyyy-MM-dd HH:mm:ss,fff}[{logEvent.Level}]: {logEvent.RenderedMessage} \r\n");
        }
    }
}
