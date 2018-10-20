using System;

namespace Cama.Service.Exceptions
{
    public class OpenProjectException : Exception
    {
        public OpenProjectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
