using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser.Metrics
{
    internal class SimpleMetric : ISimpleMetric
    {
        public string Type { get; }
        public string Name { get; }
        public string Value { get; }

        public SimpleMetric(string type,string name,string value)
        {
            Type = type;
            Name = name;
            Value = value;
        }
    }
}
