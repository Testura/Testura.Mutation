using System;
using System.Collections.Generic;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Execution.Runners;
using Testura.Mutation.Tests.Utils.Stubs;

namespace Testura.Mutation.Tests.Utils.Creators
{
    public static class TestRunnerClientCreator
    {
        public static ITestRunnerClient CreatePositive()
        {
            return new TestRunnerClientStub(new TestSuiteResult
            {
                IsSuccess = true,
                ExecutionTime = TimeSpan.FromSeconds(1),
                TestResults = new List<TestResult>
                {
                    new TestResult {IsSuccess = true}
                }
            });
        }

        public static ITestRunnerClient CreateNegative()
        {
            return new TestRunnerClientStub(new TestSuiteResult
            {
                IsSuccess = false,
                ExecutionTime = TimeSpan.FromSeconds(1),
                TestResults = new List<TestResult>
                {
                    new TestResult {IsSuccess = false}
                }
            });
        }
    }
}
