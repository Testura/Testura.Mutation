using System.Collections.Generic;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Testura.Mutation.Application.Commands.Mutation.ExecuteMutations;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Tests.Application.Commands.Mutation.ExecuteMutations
{
    [TestFixture]
    public class ExecuteMutationsCommandValidatorTests
    {
        private ExecuteMutationsCommandValidator _executeMutationsCommandValidator;

        [OneTimeSetUp]
        public void SetUp()
        {
            _executeMutationsCommandValidator = new ExecuteMutationsCommandValidator();
        }

        [Test]
        public void Validation_WhenConfigIsNull_ShouldThrowError()
        {
            _executeMutationsCommandValidator.ShouldHaveValidationErrorFor(command => command.Config, new ExecuteMutationsCommand(null, new List<MutationDocument>()));
        }

        [Test]
        public void Validation_WhenMutationDocumentIsNull_ShouldThrowError()
        {
            _executeMutationsCommandValidator.ShouldHaveValidationErrorFor(command => command.MutationDocuments, new ExecuteMutationsCommand(new MutationConfig(), null));
        }

        [Test]
        public void Validation_WhenMutationNumberOfTestRunInstancesIsLessThan1_ShouldThrowError()
        {
            _executeMutationsCommandValidator.ShouldHaveValidationErrorFor(command => command.Config.NumberOfTestRunInstances, new ExecuteMutationsCommand(new MutationConfig { NumberOfTestRunInstances = 0}, null));
        }

        [Test]
        public void Validation_WhenMutationNumberOfTestRunInstancesIsMoreThan0_ShouldNotThrowError()
        {
            _executeMutationsCommandValidator.ShouldNotHaveValidationErrorFor(command => command.Config.NumberOfTestRunInstances, new ExecuteMutationsCommand(new MutationConfig { NumberOfTestRunInstances = 1 }, null));
        }

        [Test]
        public void Validation_WhenTestProjectIsEmpty_ShouldThrowError()
        {
            _executeMutationsCommandValidator.ShouldHaveValidationErrorFor(command => command.Config.TestProjects, new ExecuteMutationsCommand(new MutationConfig(), null));
        }

        [Test]
        public void Validation_WhenSolutionIsNull_ShouldThrowError()
        {
            _executeMutationsCommandValidator.ShouldHaveValidationErrorFor(command => command.Config.Solution, new ExecuteMutationsCommand(new MutationConfig(), null));
        }
    }
}
