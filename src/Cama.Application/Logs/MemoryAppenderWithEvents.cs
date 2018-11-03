using System;
using log4net.Appender;

namespace Cama.Application.Logs
{
    public class MemoryAppenderWithEvents : MemoryAppender
    {
        public event EventHandler<log4net.Core.LoggingEvent> Updated;

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            // Append the event as usual
            base.Append(loggingEvent);

            Updated?.Invoke(this, loggingEvent);
        }
    }
}
