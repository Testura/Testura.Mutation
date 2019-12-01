using System;
using System.IO;
using System.Net;

namespace Unima.Git.Services
{
    public class SolutionFinderService
    {
        public string FindSolutionPath(string path)
        {
            var sln = LookInFiles(path);

            if(sln != null)
            {
                return sln;
            }

            throw new Exception("WHAT WHAT");
        }

        private string LookInFiles(string path)
        {
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                if (file.EndsWith("sln"))
                {
                    return file;
                }
            }

            return null;
        }
    }
}
