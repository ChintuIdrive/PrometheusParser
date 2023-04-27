using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser.Metrics
{
    internal interface IMultiLabelMatric:IMetric
    {
        List<DataPoint> DataPoints { get; }

        void Add(DataPoint dataPoint);
    }
}
