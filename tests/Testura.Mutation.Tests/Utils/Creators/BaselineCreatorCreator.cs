using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testura.Mutation.Core.Baseline;
using Testura.Mutation.Core.Baseline.Handlers;
using Testura.Mutation.Core.Execution;

namespace Testura.Mutation.Tests.Utils.Creators
{
    public static class BaselineCreatorCreator
    {
        public static BaselineCreator CreatePositiveBaseline(IFileSystem fileSystem)
        {
            return new BaselineCreator(
                fileSystem,
                new BaselineCreatorCompileMutationProjectsHandler(ProjectCompilerCreator.CreatePositiveCompiler(fileSystem), fileSystem),
                new BaselineCreatorRunUnitTestsHandler(TestRunnerClientCreator.CreatePositive(), new TestRunnerDependencyFilesHandler(fileSystem)),
                new BaselineCreatorLogSummaryHandler());
        }

        public static BaselineCreator CreateBaselineWithCompileFail(IFileSystem fileSystem)
        {
            return new BaselineCreator(
                fileSystem,
                new BaselineCreatorCompileMutationProjectsHandler(ProjectCompilerCreator.CreateNegativeCompiler(fileSystem), fileSystem),
                new BaselineCreatorRunUnitTestsHandler(TestRunnerClientCreator.CreatePositive(), new TestRunnerDependencyFilesHandler(fileSystem)),
                new BaselineCreatorLogSummaryHandler());
        }

        public static BaselineCreator CreateBaselineWithTestRunFail(IFileSystem fileSystem)
        {
            return new BaselineCreator(
                fileSystem,
                new BaselineCreatorCompileMutationProjectsHandler(ProjectCompilerCreator.CreatePositiveCompiler(fileSystem), fileSystem),
                new BaselineCreatorRunUnitTestsHandler(TestRunnerClientCreator.CreateNegative(), new TestRunnerDependencyFilesHandler(fileSystem)),
                new BaselineCreatorLogSummaryHandler());
        }
    }
}
