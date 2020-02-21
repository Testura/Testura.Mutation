using System.Collections.Generic;
using log4net;

namespace Testura.Mutation.Core.Config
{
    public class TargetFramework
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TargetFramework));

        public string Name { get; set; }

        public bool IgnoreProjectsWithWrongTargetFramework { get; set; }

        public Dictionary<string, string> CreateProperties()
        {
            var props = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(Name))
            {
                Log.Info($"Found a target framework in config: {Name}. Adding it to properties");
                props.Add("TargetFramework", Name);
            }

            return props;
        }
    }
}
