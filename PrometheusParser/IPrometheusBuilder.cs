using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal interface IPrometheusBuilder
    {
        PrometheusResponse Build(string[] lines);
    }
}
