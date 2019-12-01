using System.IO;
using Anotar.Log4Net;
using LibGit2Sharp;
using Unima.Core.Git;

namespace Unima.Infrastructure.Git
{
    public class GitCloner : IGitCloner
    {
        public void ClonseSolution(string repositoryUrl, string branch, string username, string password, string outputPath)
        {
            if (Directory.Exists(outputPath))
            {
                LogTo.Info("Directory already exist so will delete it..");

                // It seems like the git directory sometimes are locked so force it to normal status.
                SetAttributes(new DirectoryInfo(outputPath));

                Directory.Delete(outputPath, true);
                LogTo.Info(".. deleting done.");
            }

            var co = new CloneOptions
            {
                BranchName = branch
            };

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                LogTo.Info("Found username and password in git config so will update clone options.");
                co.CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials { Username = username, Password = password };
            }

            LogTo.Info($"Cloning \"{repositoryUrl}\" (branch {branch}) to \"{outputPath}\"..");
            Repository.Clone(repositoryUrl, outputPath, co);
            LogTo.Info(".. cloning done.");
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
