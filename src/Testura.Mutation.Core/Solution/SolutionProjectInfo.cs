using System.IO;

namespace Testura.Mutation.Core.Solution
{
    public class SolutionProjectInfo
    {
        public SolutionProjectInfo(string name, string filePath, string outputFilePath)
        {
            Name = name;
            FilePath = filePath;
            OutputDirectoryPath = Path.GetDirectoryName(outputFilePath);
            OutputFileName = Path.GetFileName(outputFilePath);
        }

        public string FilePath { get; set; }

        public string Name { get; set; }

        public string OutputDirectoryPath { get; set; }

        public string OutputFileName { get; set; }
    }
}
