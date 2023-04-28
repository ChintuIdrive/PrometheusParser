using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal class ComplexMetric : SimpleMetric,IComplexMetric
    {
        public string Count { get; }

        public string Sum { get; }

        public ComplexMetric(string name, string type, string description, DataPoint dataPoint,string sum,string count):base(name, type, description, dataPoint)
        {
            Sum = sum;
            Count = count;
        }
        public override string ToString()
        {
            string simpleMetricInfo=base.ToString();
            string sumInfo = $"{Name}_sum {Sum}";
            string countInfo=$"{Name}_count {Count}";
            string complexMatricInfo = string.Join('\n', simpleMetricInfo, sumInfo,countInfo);
            return complexMatricInfo;
        }
    }
}
