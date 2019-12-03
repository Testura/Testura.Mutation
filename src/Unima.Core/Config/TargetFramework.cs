using System.Collections.Generic;
using Anotar.Log4Net;

namespace Unima.Core.Config
{
    public class TargetFramework
    {
        public string Name { get; set; }

        public bool IgnoreProjectsWithWrongTargetFramework { get; set; }

        public Dictionary<string, string> CreateProperties()
        {
            var props = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(Name))
            {
                LogTo.Info($"Found a target framework in config: {Name}. Adding it to properties");
                props.Add("TargetFramework", Name);
            }

            return props;
        }
    }
}
