using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser.Metrics
{
    internal interface IComplexMatric:IMultiLabelMatric
    {
        string Sum { get; }
        string Count { get; }
    }
}
