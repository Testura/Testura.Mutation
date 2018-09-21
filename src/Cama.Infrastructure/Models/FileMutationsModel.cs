using System.Collections.Generic;
using Cama.Core.Mutation.Models;

namespace Cama.Infrastructure.Models
{
    public class FileMutationsModel
    {
        public FileMutationsModel(string fileName, IList<MutationDocument> statementsMutations)
        {
            FileName = fileName;
            MutatedDocuments = new List<MutationDocument>(statementsMutations);
        }

        public string FileName { get; }

        public IList<MutationDocument> MutatedDocuments { get; set; }
    }
}
