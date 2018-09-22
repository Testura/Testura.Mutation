using System.Collections.Generic;
using Cama.Core;

namespace Cama.Service
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
