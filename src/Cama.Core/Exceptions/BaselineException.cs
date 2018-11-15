using System;

namespace Cama.Core.Exceptions
{
    public class BaselineException : Exception
    {
        public BaselineException(string message)
            : base(message)
        {
        }

        public BaselineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
