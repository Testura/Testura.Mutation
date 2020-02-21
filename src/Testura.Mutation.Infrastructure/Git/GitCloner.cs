using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;
using log4net;
using Testura.Mutation.Core.Git;

namespace Testura.Mutation.Infrastructure.Git
{
    public class GitCloner : IGitCloner
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GitCloner));

        public Task CloneSolutionAsync(string repositoryUrl, string branch, string username, string password, string outputPath)
        {
            return Task.Run(() =>
            {
                if (Directory.Exists(outputPath))
                {
                    Log.Info("Directory already exist so will delete it..");

                    // It seems like the git directory sometimes are locked so force it to normal status.
                    SetAttributes(new DirectoryInfo(outputPath));

                    Directory.Delete(outputPath, true);
                    Log.Info(".. deleting done.");
                }

                var co = new CloneOptions
                {
                    BranchName = branch
                };

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    Log.Info("Found username and password in git config so will update clone options.");
                    co.CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                        {Username = username, Password = password};
                }

                Log.Info($"Cloning \"{repositoryUrl}\" (branch {branch}) to \"{outputPath}\"..");
                Repository.Clone(repositoryUrl, outputPath, co);
                Log.Info(".. cloning done.");
            });
        }

        private void SetAttributes(DirectoryInfo directory)
        {
            foreach (var directoryInfo in directory.GetDirectories())
            {
                SetAttributes(directoryInfo);
            }

            foreach (var fileInfo in directory.GetFiles())
            {
                fileInfo.Attributes = FileAttributes.Normal;
            }

            directory.Attributes = FileAttributes.Normal;
        }
    }
}
