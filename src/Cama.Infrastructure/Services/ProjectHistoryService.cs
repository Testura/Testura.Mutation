using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Cama.Infrastructure.Services
{
    public class ProjectHistoryService
    {
        private const string FileName = "CamaProjectHistory.Json";
        private string HistoryPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileName);

        public void AddToHistory(string path)
        {
            var history = new List<string>();
            if (File.Exists(HistoryPath))
            {
                history = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(HistoryPath));
            }

            if (history.Contains(path))
            {
                history.Remove(path);
            }

            history.Insert(0, path);
            File.WriteAllText(HistoryPath, JsonConvert.SerializeObject(history));
        }

        public IList<string> GetHistory()
        {
            if (!File.Exists(HistoryPath))
            {
                return new List<string>();
            }

            return JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(HistoryPath));
        }


    }
}