using System;

namespace Testura.Mutation.Application.Exceptions
{
    public class OpenProjectException : Exception
    {
        public OpenProjectException(string message)
            : base(message)
        {
        }

        public OpenProjectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
