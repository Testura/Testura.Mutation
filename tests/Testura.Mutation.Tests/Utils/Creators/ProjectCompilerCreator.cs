using System.Collections.Generic;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Tests.Utils.Stubs;

namespace Testura.Mutation.Tests.Utils.Creators
{
    public static class ProjectCompilerCreator
    {
        public static IProjectCompiler CreatePositiveCompiler()
        {
            return CreateCompiler(new CompilationResult {IsSuccess = true});
        }

        public static IProjectCompiler CreateNegativeCompiler()
        {
            return CreateCompiler(new CompilationResult
                {
                    IsSuccess = false,
                    Errors = new List<CompilationError>
                        {new CompilationError {Message = "Compile message", Location = "Test.cs"}}
                });
        }

        private static IProjectCompiler CreateCompiler(CompilationResult compilerResults)
        {
            return new ProjectCompilerStub(compilerResults);
        }
    }
}
