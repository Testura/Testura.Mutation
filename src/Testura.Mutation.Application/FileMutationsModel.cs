using System.Collections.Generic;
using Testura.Mutation.Core;

namespace Testura.Mutation.Application
{
    public class FileMutationsModel
    {
        public FileMutationsModel(string fileName, IList<MutationDocument> statementsMutations)
        {
            FileName = fileName;
            MutationDocuments = new List<MutationDocument>(statementsMutations);
        }

        public string FileName { get; }

        public IList<MutationDocument> MutationDocuments { get; set; }
    }
}
