using System;

namespace Unima.Core.Exceptions
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
