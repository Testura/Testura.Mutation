using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Testura.Mutation.Application.Commands.Project.OpenProject.Handlers;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Creator.Mutators;
using Testura.Mutation.Core.Creator.Mutators.BinaryExpressionMutators;

namespace Testura.Mutation.Tests.Application.Commands.Project.OpenProject.Handlers
{
    [TestFixture]
    public class OpenProjectMutatorsHandlerTests
    {
        private OpenProjectMutatorsHandler _openProjectMutatorsHandler;

        [SetUp]
        public void SetUp()
        {
            _openProjectMutatorsHandler = new OpenProjectMutatorsHandler();
        }

        [Test]
        public void InitializeMutators_WhenMutatorOperatorsIsNull_ShouldGetDefaultMutators()
        {
            var mutators = _openProjectMutatorsHandler.InitializeMutators(null);
            Assert.AreEqual(6, mutators.Count);
        }

        [Test]
        public void InitializeMutators_WhenMutatorOperatorsIsEmpty_ShouldGetDefaultMutators()
        {
            var mutators = _openProjectMutatorsHandler.InitializeMutators(new List<string>());
            Assert.AreEqual(6, mutators.Count);
        }

        [Test]
        public void InitializeMutators_WhenMutatorOperatorsContainsInvalidValues_ShouldThrowException()
        {
            Assert.Throws<OpenProjectException>(() =>_openProjectMutatorsHandler.InitializeMutators(new List<string>() { "Test"}));
        }

        [Test]
        public void InitializeMutators_WhenMutatorOperatorsContainsVvalidValues_ShouldGetMutator()
        {
            var mutators = _openProjectMutatorsHandler.InitializeMutators(new List<string>() { MutationOperators.ConditionalBoundary.ToString(), MutationOperators.Increment.ToString() });

            Assert.AreEqual(2, mutators.Count);
            Assert.IsTrue(mutators.Any(m => m.GetType() == typeof(ConditionalBoundaryMutator)));
            Assert.IsTrue(mutators.Any(m => m.GetType() == typeof(IncrementsMutator)));
        }

        [Test]
        public void InitializeMutators_WhenSendingTokenThatIsCancelled_ShouldCancel()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            tokenSource.Cancel();

            Assert.Throws<OperationCanceledException>(() => _openProjectMutatorsHandler.InitializeMutators(new List<string>(), token));
        }
    }
}
