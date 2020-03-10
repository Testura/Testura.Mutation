using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Loggers;

namespace Testura.Mutation.Tests.Utils.Stubs
{
    public class MutationRunLoggerStub : IMutationRunLogger
    {
        public void LogBeforeRun(IList<MutationDocument> mutationDocuments)
        {
            throw new NotImplementedException();
        }

        public void LogBeforeMutation(MutationDocument mutationDocument)
        {
            throw new NotImplementedException();
        }

        public void LogAfterMutation(MutationDocument mutationDocument, List<MutationDocumentResult> results, int mutationsRemainingCount)
        {
            throw new NotImplementedException();
        }
    }
}
