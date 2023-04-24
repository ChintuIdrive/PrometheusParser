using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal class DataPoint
    {
        private readonly Dictionary<string, string> _labels;
        private readonly string _value;
        public DataPoint(Dictionary<string, string> labes,string value) 
        { 
            _labels = labes;
            _value = value;
        }

        public Dictionary<string, string> Labels => _labels;

        public string Value => _value;
    }
}
