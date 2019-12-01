using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medallion.Shell.Streams;

namespace Unima.Infrastructure.Stream
{
    public abstract class StreamReaderBase
    {
        protected bool ReadToEnd(ProcessStreamReader processStream, out string message)
        {
            var readStreamTask = Task.Run(() => processStream.ReadToEnd());
            var successful = readStreamTask.Wait(TimeSpan.FromSeconds(30));

            message = successful ? readStreamTask.Result : "Error reading from stream";
            return successful;
        }
    }
}
