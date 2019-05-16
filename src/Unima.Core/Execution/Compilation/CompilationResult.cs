using System.Collections.Generic;

namespace Unima.Core.Execution.Compilation
{
    public class CompilationResult
    {
        public bool IsSuccess { get; set; }

        public IList<CompilationError> Errors { get; set; }
    }
}
