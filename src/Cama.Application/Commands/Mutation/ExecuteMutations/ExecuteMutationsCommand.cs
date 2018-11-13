using System;
using System.Collections.Generic;
using Cama.Core;
using MediatR;

namespace Cama.Application.Commands.Mutation.ExecuteMutations
{
    public class ExecuteMutationsCommand : IRequest<IList<MutationDocumentResult>>
    {
        public ExecuteMutationsCommand(
            CamaConfig config,
            IList<MutationDocument> mutationDocuments,
            Action<MutationDocument> mutationDocumentStartedCallback = null,
            Action<MutationDocumentResult> mutationDocumentCompledtedCallback = null)
        {
            Config = config;
            MutationDocuments = mutationDocuments;
            MutationDocumentStartedCallback = mutationDocumentStartedCallback;
            MutationDocumentCompledtedCallback = mutationDocumentCompledtedCallback;
        }

        public CamaConfig Config { get; }

        public IList<MutationDocument> MutationDocuments { get; }

        public Action<MutationDocument> MutationDocumentStartedCallback { get; }

        public Action<MutationDocumentResult> MutationDocumentCompledtedCallback { get; }
    }
}
