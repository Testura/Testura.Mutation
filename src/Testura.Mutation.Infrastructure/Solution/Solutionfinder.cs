using System.IO;
using System.Linq;

namespace Testura.Mutation.Infrastructure.Solution
{
    public class SolutionFinder
    {
        public string FindSolution(string directoryPath)
        {
           var files =  Directory.GetFiles(directoryPath, "*.sln");

           if (files.Any())
           {
               return files.First();
           }

           foreach (var directory in Directory.GetDirectories(directoryPath))
           {
               var path = FindSolution(directoryPath);

               if (path != null)
               {
                   return path;
               }
           }

           return null;
        }
    }
}
