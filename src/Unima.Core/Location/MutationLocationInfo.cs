using System;

namespace Unima.Core.Location
{
    public class MutationLocationInfo
    {
        public string Line { get; set; }

        public string Where { get; set; }

        public int GetLineNumber()
        {
            return int.Parse(Line.Split(new[] { "@(", ":" }, StringSplitOptions.RemoveEmptyEntries)[0]);
        }

        public override string ToString()
        {
            return $"{Where}({Line})";
        }
    }
}
