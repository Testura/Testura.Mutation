﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Testura.Mutation.Core.Execution.Result
{
    public class MutationRunResult
    {
        public MutationRunResult(IList<MutationDocumentResult> mutationDocumentResults, bool wasCancelled)
        {
            MutationDocumentResults = mutationDocumentResults;
            WasCancelled = wasCancelled;
        }

        public IList<MutationDocumentResult> MutationDocumentResults { get; }

        public bool WasCancelled { get; }

        public bool Success => MutationDocumentResults.All(r => !r.Survived);

        public double GetMutationScore()
        {
            var validMutations = MutationDocumentResults.Where(r => r.CompilationResult != null && r.CompilationResult.IsSuccess && r.UnexpectedError == null).ToList();

            if (!validMutations.Any())
            {
                return 100;
            }

            return Math.Round((double)validMutations.Count(r => !r.Survived) / validMutations.Count * 100);
        }
    }
}
