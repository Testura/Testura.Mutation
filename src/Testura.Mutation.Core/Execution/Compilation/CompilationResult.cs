using System.Collections.Generic;

namespace Testura.Mutation.Core.Execution.Compilation
{
    public class CompilationResult
    {
        public bool IsSuccess { get; set; }

        public IList<CompilationError> Errors { get; set; }
    }
}
