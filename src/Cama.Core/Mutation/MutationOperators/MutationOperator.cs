using System.Collections.Generic;
using System.Linq;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cama.Core.Mutation.MutationOperators
{
    public class MutationOperator : CSharpSyntaxRewriter, IMutationOperator
    {
        public MutationOperator()
        {
            Replacers = new List<Replacer>();
        }

        protected IList<Replacer> Replacers { get; }

        public IList<MutatedDocument> GetMutatedDocument(SyntaxNode root, Document document, List<UnitTestInformation> connectedTests)
        {
            Replacers.Clear();
            Visit(root);
            return Replacers.Select(r => new MutatedDocument(document, r, connectedTests)).ToList();
        }
    }
}
