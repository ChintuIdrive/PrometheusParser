using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal class Parser
    {
        // Create a dictionary to store the metrics
        private readonly Dictionary<string, ISimpleMetric> _simpleMetrics;
        private readonly Dictionary<string, IComplexMetric> _complexMetrics;
        public Parser()
        {
            _simpleMetrics = new Dictionary<string, ISimpleMetric>();
            _complexMetrics= new Dictionary<string, IComplexMetric>();
        }
        public void Load(string PromResponseFilePath)
        {
            List<string[]> rawMetrics = GetRawMetrics(PromResponseFilePath);
            foreach (string[] metrics in rawMetrics)
            {
                Parse(metrics);
            }
        }
        public IEnumerable<KeyValuePair<string, ISimpleMetric>> GetSimpleMerics()
        {
            return _simpleMetrics; 
        }
        public IEnumerable<KeyValuePair<string, IComplexMetric>> GetComplexMerics()
        {
            return _complexMetrics;
        }
        public IEnumerable<KeyValuePair<string, ISimpleMetric>> GetGaugeMetrics()
        {
            return _simpleMetrics.Where(x => x.Value.Type == "gauge");
        }
        public IEnumerable<KeyValuePair<string, ISimpleMetric>> GetCounterMetrics()
        {
            return _simpleMetrics.Where(x => x.Value.Type == "counter");
        }
        public IEnumerable<KeyValuePair<string, IComplexMetric>> GetSummaryMetrics()
        {
            return _complexMetrics.Where(x => x.Value.Type == "summary");
        }
        public IEnumerable<KeyValuePair<string, IComplexMetric>> GetHistogramMetrics()
        {
            return _complexMetrics.Where(x => x.Value.Type == "histogram");
        }
        public IComplexMetric GetComplexMetric(string metricName)
        {
            return _complexMetrics.SingleOrDefault(x => x.Key == metricName).Value;
        }
        public ISimpleMetric GetSimpleMetric(string metricName)
        {
            return _simpleMetrics.SingleOrDefault(x=>x.Key == metricName).Value;
        }
        private void Parse(string[] rawMetricLines)
        {
            string helpLine = rawMetricLines[0];
            string typeLine = rawMetricLines[1];
            //split typeLine to extract name and type
            string[] parts = typeLine.Split(' ');
            string name = parts[2];
            string type = parts[3];
            if (type == "counter" || type == "gauge")
            {
                for (int i = 2; i < rawMetricLines.Length; i++)
                {
                    string metricLine = rawMetricLines[i];
                    DataPoint dataPoint = GetDataPoint(metricLine);
                    // Add the metric to the dictionary               
                    if (_simpleMetrics.ContainsKey(name))
                        _simpleMetrics[name].AddDataPoint(dataPoint);
                    else
                        _simpleMetrics.Add(name, new SimpleMetric(name, type,helpLine,dataPoint));
                }
            }
            else if (type == "summary" || type == "histogram")
            {
                string sumline = rawMetricLines[rawMetricLines.Length - 2];
                string[] sumParts = sumline.Split(" ");
                string sum= sumParts[1];

                string countLine = rawMetricLines[rawMetricLines.Length - 1];
                string[] countParts = countLine.Split(" ");
                string count= countParts[1];

                for (int i = 2; i < rawMetricLines.Length-2; i++)
                {
                    string metricLine= rawMetricLines[i];
                    DataPoint dataPoint = GetDataPoint(metricLine);
                    // Add the metric to the dictionary               
                    if (_complexMetrics.ContainsKey(name))
                        _complexMetrics[name].AddDataPoint(dataPoint);
                    else
                        _complexMetrics.Add(name, new ComplexMetric(name, type, helpLine, dataPoint, sum,count));
                }
            }
            
        }
        public List<string[]> GetRawMetrics(string PromResponseFilePath)
        {
            List<string[]> rawMetrics = new List<string[]>();
            // Read the file and split the response into lines         
            string[] lines = File.ReadAllLines(PromResponseFilePath).Skip(1).ToArray();

            // Loop through the lines and parse the metrics
            int commentCounter = 0;
            List<string> rawMetricLines = new List<string>();

            foreach (string line in lines)
            {
                // Skip the empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("#"))
                {
                    ++commentCounter;
                    if (commentCounter == 1)
                    {
                        rawMetricLines = new List<string>();
                        rawMetricLines.Add(line);
                    }
                    else if (commentCounter > 1 && commentCounter < 3)
                    {
                        rawMetricLines.Add(line);
                    }
                    else if (commentCounter >= 3)
                    {
                        string[] temp = rawMetricLines.ToArray<string>();
                        rawMetrics.Add(temp);
                        rawMetricLines = new List<string>();
                        rawMetricLines.Add(line);
                        commentCounter = 1;
                    }
                }
                else
                {
                    rawMetricLines.Add(line);
                }

            }

            return rawMetrics;
        }
        private DataPoint GetDataPoint(string line)
        {
            Dictionary<string, string> labels = new Dictionary<string, string>();
            // Split the line into metric name, labels, and value
            string[] parts = line.Split(' ');
            string value = parts[1];
            string pattern = @"\{([^}]*)\}";
            Match match = Regex.Match(line, pattern);
            if (match.Success)
            {
                string result = match.Groups[1].Value;
                string[] rawlabels = result.Split(",");
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
