using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Cama.Core.Creator.Mutators
{
    public interface IMutator
    {
        IList<MutationDocument> GetMutatedDocument(SyntaxNode root, Document document);
    }
}