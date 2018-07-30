using System.Collections.Generic;
using Cama.Core.Models.Mutation;

namespace Cama.Core.Models
{
    public class MFile
    {
        public MFile(string fileName, IList<MutatedDocument> statementsMutations)
        {
            FileName = fileName;
            StatementsMutations = new List<MutatedDocument>(statementsMutations);
        }

        public string FileName { get; }

        public IList<MutatedDocument> StatementsMutations { get; set; }
    }
}
