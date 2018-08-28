using System;

namespace Cama.Core.Exceptions
{
    public class ProjectSetUpException : Exception
    {
        public ProjectSetUpException(string message)
            : base(message)
        {
        }
    }
}
