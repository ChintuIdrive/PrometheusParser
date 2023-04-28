using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal interface IComplexMetric:ISimpleMetric
    {
        string Count { get; }
        string Sum { get; }
    }
}
