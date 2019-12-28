using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Testura.Mutation.Core.Creator.Mutators
{
    public interface IMutator
    {
        IList<MutationDocument> GetMutatedDocument(SyntaxNode root, Document document);
    }
}