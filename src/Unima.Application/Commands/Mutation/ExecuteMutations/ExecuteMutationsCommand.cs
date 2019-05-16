using System;
using System.Collections.Generic;
using MediatR;
using Unima.Core;
using Unima.Core.Config;

namespace Unima.Application.Commands.Mutation.ExecuteMutations
{
    public class ExecuteMutationsCommand : IRequest<IList<MutationDocumentResult>>
    {
        public ExecuteMutationsCommand(
            UnimaConfig config,
            IList<MutationDocument> mutationDocuments,
            Action<MutationDocument> mutationDocumentStartedCallback = null,
            Action<MutationDocumentResult> mutationDocumentCompledtedCallback = null)
        {
            Config = config;
            MutationDocuments = mutationDocuments;
            MutationDocumentStartedCallback = mutationDocumentStartedCallback;
            MutationDocumentCompledtedCallback = mutationDocumentCompledtedCallback;
        }

        public UnimaConfig Config { get; }

        public IList<MutationDocument> MutationDocuments { get; }

        public Action<MutationDocument> MutationDocumentStartedCallback { get; }

        public Action<MutationDocumentResult> MutationDocumentCompledtedCallback { get; }
    }
}
