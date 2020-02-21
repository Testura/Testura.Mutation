using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Testura.Mutation.Core.Creator.Filter
{
    public class MutationDocumentFilterCodeItem
    {
        public string Code { get; set; }

        public bool CodeAreAllowed(SyntaxNode orginalCode)
        {
            var code = FormattedCode();
            return !Regex.IsMatch(orginalCode.ToString(), code);
        }

        private string FormattedCode()
        {
            return "^" + Regex.Escape(Code).Replace("\\*", ".*") + "$";
        }
    }
}
