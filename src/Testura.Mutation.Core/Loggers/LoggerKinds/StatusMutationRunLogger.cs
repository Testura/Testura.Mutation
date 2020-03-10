using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Testura.Mutation.Core.Loggers.LoggerKinds
{
    public class StatusMutationRunLogger : IMutationRunLogger
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StatusMutationRunLogger));

        public void LogBeforeRun(IList<MutationDocument> mutationDocuments)
        {
        }

        public void LogBeforeMutation(MutationDocument mutationDocument)
        {
        }

        public void LogAfterMutation(MutationDocument mutationDocument, List<MutationDocumentResult> results, int mutationsRemainingCount)
        {
            var survived = results.Count(r => r.Survived && (r.CompilationResult != null && r.CompilationResult.IsSuccess) && r.UnexpectedError == null);
            var killed = results.Count(r => !r.Survived && (r.CompilationResult != null && r.CompilationResult.IsSuccess) && r.UnexpectedError == null);
            var compileErrors = results.Count(r => r.CompilationResult != null && !r.CompilationResult.IsSuccess);
            var unknownErrors = results.Count(r => r.UnexpectedError != null);

            Log.Info($"Current progress: {{ Survived: {survived}, Killed: {killed}, CompileErrors: {compileErrors}, UnknownErrors: {unknownErrors}, MutationsLeft: {mutationsRemainingCount} }}");
        }
    }
}
