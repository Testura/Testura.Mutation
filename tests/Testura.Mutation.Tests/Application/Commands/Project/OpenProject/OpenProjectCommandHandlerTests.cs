using Moq;
using NUnit.Framework;
using Testura.Mutation.Application.Commands.Project.OpenProject;
using Testura.Mutation.Application.Commands.Project.OpenProject.Handlers;
using Testura.Mutation.Core.Baseline;
using Testura.Mutation.Core.Creator.Filter;
using Testura.Mutation.Core.Git;

namespace Testura.Mutation.Tests.Application.Commands.Project.OpenProject
{
    [TestFixture]
    public class OpenProjectCommandHandlerTests
    {
        public void Do()
        {
            /*
            var o = new OpenProjectCommandHandler(
                    new OpenProjectSolutionExistHandler(),
                    new OpenProjectMutatorsHandler(),
                    new OpenProjectGitFilterHandler(new MutationDocumentFilterItemGitDiffCreator(new Mock<IGitDiff>().Object)),
                    new OpenProjectWorkspaceHandler(new BaselineCreator(), ))
                    */
        }
    }
}
