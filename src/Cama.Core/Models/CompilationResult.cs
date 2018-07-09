using System.Collections.Generic;

namespace Cama.Core.Models
{
    public class CompilationResult
    {
        public bool IsSuccess { get; set; }

        public IList<CompilationError> Errors { get; set; }
    }
}
