using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace Cama.Core.Models.Mutation
{
    public class MutatedDocument
    {
        private readonly Document _orginalDocument;
        private readonly IList<UnitTestInformation> _tests;

        public MutatedDocument(Document orginalDocument, MutationInfo mutationInfo, IList<UnitTestInformation> tests)
        {
            MutationInfo = mutationInfo;
            Id = Guid.NewGuid();
            FileName = orginalDocument?.Name;
            ProjectName = orginalDocument?.Project.Name;
            _orginalDocument = orginalDocument;
            _tests = tests;
        }

        public Guid Id { get; }

        public string FileName { get;  }

        public string ProjectName { get; set; }

        public MutationInfo MutationInfo { get; }

        public IList<string> Tests => _tests.Select(t => t.TestName).ToList();

        public string MutationName => $"{ProjectName}.{FileName}({MutationInfo.Location.Where} - {MutationInfo.Location.Line})";

        public Document CreateMutatedDocument()
        {
            var editor = DocumentEditor.CreateAsync(_orginalDocument).Result;
            editor.ReplaceNode(MutationInfo.Orginal, MutationInfo.Mutation);
            return _orginalDocument.WithText(editor.GetChangedDocument().GetSyntaxRootAsync().Result.GetText());
        }
    }
}