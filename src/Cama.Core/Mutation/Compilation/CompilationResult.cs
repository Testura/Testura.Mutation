using System.Collections.Generic;

namespace Cama.Core.Mutation.Compilation
{
    public class CompilationResult
    {
        public bool IsSuccess { get; set; }

        public IList<CompilationError> Errors { get; set; }
    }
}
