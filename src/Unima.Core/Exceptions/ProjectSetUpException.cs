using System;

namespace Unima.Core.Exceptions
{
    public class ProjectSetUpException : Exception
    {
        public ProjectSetUpException(string message)
            : base(message)
        {
        }
    }
}
