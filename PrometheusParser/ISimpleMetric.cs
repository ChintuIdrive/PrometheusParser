using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal interface ISimpleMetric
    {
        string Name { get; }
        string Type { get; }
        string Description { get; }
        List<DataPoint> DataPoints { get; }
        void AddDataPoint(DataPoint dataPoint);
    }
}
