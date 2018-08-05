using System.Collections.Generic;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;

namespace Cama.Core.Mutation.MutationOperators.StatementMutations
{
    public class MathMutationOperator : IMutationOperator
    {
        public IList<MutatedDocument> GetMutatedDocument(SyntaxNode root, Document document, List<UnitTestInformation> connectedTests)
        {
            return null;
        }
    }
}
