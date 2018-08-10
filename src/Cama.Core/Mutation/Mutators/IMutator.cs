using System.Collections.Generic;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;

namespace Cama.Core.Mutation.Mutators
{
    public interface IMutator
    {
        IList<MutatedDocument> GetMutatedDocument(SyntaxNode root, Document document, List<UnitTestInformation> connectedTests);
    }
}