using System;
using System.Threading.Tasks;
using LibGit2Sharp;
using Unima.Git.Services;

namespace Unima.Git.Commands
{
    public class MutateGitCommand : ICommand
    {
        private readonly string _repositoryUrl;
        private readonly string _branch;
        private readonly string _username;
        private readonly string _password;
        private readonly string _outputPath;

        private NugetRestoreService _nugetRestoreService;
        private SolutionFinderService _solutionFinderService;
        private DotNetBuildService _dotNetBuildService;

        public MutateGitCommand(string repositoryUrl, string branch, string username, string password, string outputPath)
        {
            _repositoryUrl = repositoryUrl;
            _branch = branch;
            _username = username;
            _password = password;
            _outputPath = outputPath;

            _nugetRestoreService = new NugetRestoreService();
            _solutionFinderService = new SolutionFinderService();
            _dotNetBuildService = new DotNetBuildService();
        }

        public async Task<int> RunAsync()
        {
            var co = new CloneOptions()
            {
                BranchName = _branch
            };

            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                co.CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials { Username = _username, Password = _password };
            }

            Console.WriteLine($"Cloning \"{_repositoryUrl}\" to \"{_outputPath}\"");

            Repository.Clone(_repositoryUrl, _outputPath, co);

            _dotNetBuildService.BuildSolution(_outputPath);

            return await Task.FromResult(1);
        }
    }
}
