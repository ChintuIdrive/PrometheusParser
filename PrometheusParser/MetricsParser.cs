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
        public MetricsParser() { }
        public MetricsParser(string[] metrics) { }

        public void Parse()
        {
            // Read the file and split the response into lines
            string PromResponseFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "PromResponses", "prom_response.txt");

            string response = File.ReadAllText(PromResponseFilePath);
            string[] lines = response.Split('\n').Skip(1).ToArray();

            // Create a dictionary to store the metrics
            Dictionary<string, Metric> metrics = new Dictionary<string, Metric>();
            // Loop through the lines and parse the metrics
            foreach (string line in lines)
            {
                // Skip the comments and empty lines
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                    continue;

                // Split the line into metric name, labels, and value
                string[] parts = line.Split(' ');

                // Extract the metric name and labels
                string[] nameAndLabels = parts[0].Split('{');
                string name = nameAndLabels[0];
                DataPoint dataPoint = GetDataPoint(line);
              
                // Add the metric to the dictionary               
                if (metrics.ContainsKey(name))
                    metrics[name].AddDataPoint(dataPoint);
                else
                    metrics.Add(name, new Metric(name, dataPoint));
            }

            // Print the metrics
            foreach (Metric metric in metrics.Values)
            {
                Console.WriteLine(metric.ToString());
            }
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
