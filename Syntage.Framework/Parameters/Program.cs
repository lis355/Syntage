using System.Collections.Generic;

namespace Syntage.Framework.Parameters
{
    public class Program
    {
        public string Name { get; set; }
        public readonly Dictionary<string, double> Parameters = new Dictionary<string, double>();
    }
}
