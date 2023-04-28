using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal class Mapper
    {
        internal void MapSimpleType(IEnumerable<KeyValuePair<string, ISimpleMetric>> simpleMetrics)
        {
            foreach (var simpleMetric in simpleMetrics)
            {
                switch (simpleMetric.Key)
                {
                    case "go_goroutines":
                        // Code to execute if expression equals value1
                        break;
                    case "":
                        // Code to execute if expression equals value2
                        break;
                    // more cases can be added here
                    default:
                        // Code to execute if expression doesn't match any case
                        break;
                }
            }
        }
    }
}
