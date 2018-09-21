using System.Collections.Generic;
using Cama.Core.Mutation.Models;
using Microsoft.CodeAnalysis;

namespace Cama.Core.Mutators
{
    public interface IMutator
    {
        IList<MutationDocument> GetMutatedDocument(SyntaxNode root, Document document);
    }
}