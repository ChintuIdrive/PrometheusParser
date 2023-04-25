using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal class MetricsParser
    {
        // Create a dictionary to store the metrics
        private readonly Dictionary<string, Metric> _metrics;
        public MetricsParser() 
        {
            _metrics = new Dictionary<string, Metric>();
        }

        public void Parse(string PromResponseFilePath)
        {
            // Read the file and split the response into lines         

            string[] lines = File.ReadAllLines(PromResponseFilePath).Skip(1).ToArray(); ;
           
            // Loop through the lines and parse the metrics
            foreach (string line in lines)
            {
                // Skip the comments and empty lines
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                    continue;

                // Split the line into metric name, labels, and value
                string[] parts = line.Split(' ');

                // Extract the metric name 
                string[] nameAndLabels = parts[0].Split('{');
                string name = nameAndLabels[0];

                //Extract data point(label and value )
                DataPoint dataPoint = GetDataPoint(line);
              
                // Add the metric to the dictionary               
                if (_metrics.ContainsKey(name))
                    _metrics[name].AddDataPoint(dataPoint);
                else
                    _metrics.Add(name, new Metric(name, dataPoint));
            }
        }

        public IDictionary<string,Metric> GetMetrics()
        {
            return _metrics;
        }
        private DataPoint GetDataPoint(string line)
        {
            Dictionary<string,string> labels = new Dictionary<string,string>();
            // Split the line into metric name, labels, and value
            string[] parts = line.Split(' ');
            string value= parts[1];
            string pattern = @"\{([^}]*)\}";
            Match match = Regex.Match(line, pattern);
            if (match.Success)
            {
                string result = match.Groups[1].Value;
                string[] rawlabels=result.Split(",");
                foreach (string label in rawlabels)
                {
                    string[] labelData = label.Split("=");
                    if (!labels.ContainsKey(label))
                    {
                        labels.Add(labelData[0], labelData[1]);
                    }
                }
            }
            return new DataPoint(labels, value);
        }
    }
}
