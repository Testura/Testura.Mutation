using System;
using System.Collections.Generic;

namespace Cama.Business.Exceptions
{
    public class CompilationException : Exception
    {
        public CompilationException(IEnumerable<string> errorMessages)
        {
            ErrorMessages = new List<string>(errorMessages);
        }

        public IReadOnlyList<string> ErrorMessages { get; set; }
    }
}
