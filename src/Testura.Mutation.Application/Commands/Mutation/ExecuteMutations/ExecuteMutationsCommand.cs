using System;
using System.Collections.Generic;
using MediatR;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Application.Commands.Mutation.ExecuteMutations
{
    public class ExecuteMutationsCommand : IRequest<IList<MutationDocumentResult>>
    {
        public ExecuteMutationsCommand(
            TesturaMutationConfig config,
            IList<MutationDocument> mutationDocuments,
            Action<MutationDocument> mutationDocumentStartedCallback = null,
            Action<MutationDocumentResult> mutationDocumentCompledtedCallback = null)
        {
            Config = config;
            MutationDocuments = mutationDocuments;
            MutationDocumentStartedCallback = mutationDocumentStartedCallback;
            MutationDocumentCompledtedCallback = mutationDocumentCompledtedCallback;
        }

        public TesturaMutationConfig Config { get; }

        public IList<MutationDocument> MutationDocuments { get; }

        public Action<MutationDocument> MutationDocumentStartedCallback { get; }

        public Action<MutationDocumentResult> MutationDocumentCompledtedCallback { get; }
    }
}
