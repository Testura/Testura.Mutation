using System;
using Testura.Mutation.Core.Git;

namespace Testura.Mutation.Tests.Utils.Stubs
{
    public class GitDiffStub : IGitDiff
    {
        private readonly string _diff;

        public GitDiffStub(string diff)
        {
            _diff = diff;
        }

        public GitDiffStub()
        {
            _diff = "diff --git a/src/Testura.Mutation.Application/Commands/Project/OpenProject/Handlers/OpenProjectGitFilterHandler.cs b/src/Testura.Mutation.Application/Commands/Project/OpenProject/Handlers/OpenProjectGitFilterHandler.cs\nindex e79719c..a8baa65 100644\n--- a/src/Testura.Mutation.Application/Commands/Project/OpenProject/Handlers/OpenProjectGitFilterHandler.cs\n+++ b/src/Testura.Mutation.Application/Commands/Project/OpenProject/Handlers/OpenProjectGitFilterHandler.cs\n@@ -22 +22 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-        public void InitializeGitFilter(MutationFileConfig fileConfig, MutationConfig applicationConfig, CancellationToken cancellationToken = default(CancellationToken))\n+        public void InitializeGitFilter(string solutionPath, GitInfo gitInfo, MutationConfig applicationConfig, CancellationToken cancellationToken = default(CancellationToken))\n@@ -26 +26 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-            if (fileConfig.Git != null && fileConfig.Git.GenerateFilterFromDiffWithMaster)\n+            if (gitInfo != null && gitInfo.GenerateFilterFromDiffWithMaster)\n@@ -30 +30 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-                var filterItems = _diffCreator.GetFilterItemsFromDiff(Path.GetDirectoryName(fileConfig.SolutionPath), string.Empty);\n+                var filterItems = _diffCreator.GetFilterItemsFromDiff(Path.GetDirectoryName(solutionPath), string.Empty);\ndiff --git a/src/Testura.Mutation.Application/Commands/Project/OpenProject/Handlers/OpenProjectMutatorsHandler.cs b/src/Testura.Mutation.Application/Commands/Project/OpenProject/Handlers/OpenProjectMutatorsHandler.cs\nindex c8e7006..d662a83 100644\n--- a/src/Testura.Mutation.Application/Commands/Project/OpenProject/Handlers/OpenProjectMutatorsHandler.cs\n+++ b/src/Testura.Mutation.Application/Commands/Project/OpenProject/Handlers/OpenProjectMutatorsHandler.cs\n@@ -7 +6,0 @@ using Testura.Mutation.Application.Exceptions;\n-using Testura.Mutation.Application.Models;\n@@ -9 +7,0 @@ using Testura.Mutation.Core;\n-using Testura.Mutation.Core.Config;\n@@ -19 +17 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-        public void InitializeMutators(MutationFileConfig fileConfig, MutationConfig applicationConfig, CancellationToken cancellationToken = default(CancellationToken))\n+        public List<IMutator> InitializeMutators(List<string> mutatorOperators, CancellationToken cancellationToken = default(CancellationToken))\n@@ -25 +23 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-            if (fileConfig.Mutators == null || !fileConfig.Mutators.Any())\n+            if (mutatorOperators == null || !mutatorOperators.Any())\n@@ -27,5 +25 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-                LoadDefaultMutators(applicationConfig);\n-            }\n-            else\n-            {\n-                LoadCustomMutatorList(fileConfig, applicationConfig);\n+                return LoadDefaultMutators();\n@@ -32,0 +27,2 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n+\n+            return LoadCustomMutatorList(mutatorOperators);\n@@ -35 +31 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-        private void LoadCustomMutatorList(MutationFileConfig fileConfig, MutationConfig applicationConfig)\n+        private List<IMutator> LoadCustomMutatorList(List<string> mutatorOperators)\n@@ -39 +35 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-            foreach (var mutationOperator in fileConfig.Mutators)\n+            foreach (var mutationOperator in mutatorOperators)\n@@ -52 +48 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-            applicationConfig.Mutators = mutators;\n+            return mutators;\n@@ -55 +51 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-        private void LoadDefaultMutators(MutationConfig applicationConfig)\n+        private List<IMutator> LoadDefaultMutators()\n@@ -59 +55 @@ namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers\n-            applicationConfig.Mutators = new List<IMutator>\n+            return new List<IMutator>\n";
        }

        public string GetDiff(string path)
        {
            return _diff;
        }
    }
}
