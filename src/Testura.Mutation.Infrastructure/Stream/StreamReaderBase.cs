using System;
using System.Threading.Tasks;
using Medallion.Shell.Streams;

namespace Testura.Mutation.Infrastructure.Stream
{
    public abstract class StreamReaderBase
    {
        protected bool ReadToEnd(ProcessStreamReader processStream, out string message)
        {
            var readStreamTask = Task.Run(() => processStream.ReadToEnd());
            var successful = readStreamTask.Wait(TimeSpan.FromSeconds(180));

            message = successful ? readStreamTask.Result : "Error reading from stream";
            return successful;
        }
    }
}
