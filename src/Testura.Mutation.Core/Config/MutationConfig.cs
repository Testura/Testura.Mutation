using System.Collections.Generic;
using Testura.Mutation.Core.Baseline;
using Testura.Mutation.Core.Creator.Filter;
using Testura.Mutation.Core.Creator.Mutators;

namespace Testura.Mutation.Core.Config
{
    public class MutationConfig
    {
        public MutationConfig()
        {
            MutationProjects = new List<MutationProject>();
            TestProjects = new List<TestProject>();
            MaxTestTimeMin = 5;
            BaselineInfos = new List<BaselineInfo>();
        }

        public Microsoft.CodeAnalysis.Solution Solution { get; set; }

        public IList<MutationProject> MutationProjects { get; set; }

        public IList<TestProject> TestProjects { get; set; }

        public MutationDocumentFilter Filter { get; set; }

        public int NumberOfTestRunInstances { get; set; }

        public string BuildConfiguration { get; set; }

        public int MaxTestTimeMin { get; set; }

        public IList<BaselineInfo> BaselineInfos { get; set; }

        public string DotNetPath { get; set; }

        public IList<MutationRunLoggerKind> MutationRunLoggers { get; set; }

        public IList<IMutator> Mutators { get; set; }

        public TargetFramework TargetFramework { get; set; }
    }
}