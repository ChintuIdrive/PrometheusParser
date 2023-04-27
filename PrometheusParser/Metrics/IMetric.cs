using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser.Metrics
{
    internal interface IMetric
    {
        string Type { get; }
        string Name { get; }
    }
}
