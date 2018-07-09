namespace Cama.Core.Models.Mutation
{
    public class MutationDocumentResult
    {
        public bool Completed { get; set; }

        public CompilationResult CompilerResult { get; set; }

        public TestSuiteResult TestResult { get; set; }

        public MutatedDocument Document { get; set; }
    }
}
