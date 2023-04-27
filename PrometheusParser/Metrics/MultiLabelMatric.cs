using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser.Metrics
{
    internal class MultiLabelMatric : IMultiLabelMatric
    {
        public List<DataPoint> DataPoints { get; }

        public string Type { get; }

        public string Name { get; }

        public MultiLabelMatric(string type, string name,DataPoint dataPoint)
        {
            Type = type;
            Name = name;
            DataPoints = new List<DataPoint> { dataPoint };
        }
        public void Add(DataPoint dataPoint)
        {
            DataPoints.Add(dataPoint);
        }
    }
}
