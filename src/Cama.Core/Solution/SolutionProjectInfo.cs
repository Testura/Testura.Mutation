using System.IO;

namespace Cama.Core.Solution
{
    public class SolutionProjectInfo
    {
        public SolutionProjectInfo(string name, string outputFilePath)
        {
            Name = name;
            OutputDirectoryPath = Path.GetDirectoryName(outputFilePath);
            OutputFileName = Path.GetFileName(outputFilePath);
        }

        public string Name { get; set; }

        public string OutputDirectoryPath { get; set; }

        public string OutputFileName { get; set; }
    }
}
