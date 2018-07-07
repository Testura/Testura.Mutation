using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cama.Business.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Business.Mutation.Analyzer
{
    public class UnitTestAnalyzer
    {
        public async Task<IList<UnitTestInformation>> MapTestsAsync(Project unitTestProject)
        {
            var compilation = await unitTestProject.GetCompilationAsync();
            var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);

            if (errors.Any())
            {
                throw new CompilationException(errors.Select(e => e.ToString()));
            }

            var unitTestInformations = new List<UnitTestInformation>();
            foreach (var testProjectDocument in unitTestProject.Documents)
            {
                var semanticModel = testProjectDocument.GetSemanticModelAsync().Result;

                var root = testProjectDocument.GetSyntaxRootAsync().Result;
                var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
                foreach (var methodDeclarationSyntax in methods)
                {
                    var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
                    var test = new UnitTestInformation
                    {
                        TestName = methodSymbol.ToString().Replace("(", string.Empty).Replace(")", string.Empty)
                    };

                    var body = methodDeclarationSyntax.Body;
                    var invocations = body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                    foreach (var invocationExpressionSyntax in invocations)
                    {
                        var symbol = semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol;
                        test.ReferencedClasses.Add(symbol.ContainingType.Name);
                    }

                    unitTestInformations.Add(test);
                }
            }

            return unitTestInformations;
        }
    }
}
