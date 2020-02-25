using System.Collections.Generic;
using System.IO.Abstractions;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Tests.Utils.Stubs;

namespace Testura.Mutation.Tests.Utils.Creators
{
    public static class ProjectCompilerCreator
    {
        public static ProjectCompilerStub CreatePositiveCompiler(IFileSystem fileSystem)
        {
            return CreateCompiler(new CompilationResult {IsSuccess = true}, fileSystem);
        }

        public static ProjectCompilerStub CreateNegativeCompiler(IFileSystem fileSystem)
        {
            return CreateCompiler(new CompilationResult
                {
                    IsSuccess = false,
                    Errors = new List<CompilationError>
                        {new CompilationError {Message = "Compile message", Location = "Test.cs"}}
                }, fileSystem);
        }

        private static ProjectCompilerStub CreateCompiler(CompilationResult compilerResults, IFileSystem fileSystem)
        {
            return new ProjectCompilerStub(compilerResults, fileSystem);
        }
    }
}
