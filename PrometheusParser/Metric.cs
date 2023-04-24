using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PrometheusParser
{
    internal class Metric
    {
        public string Name { get; }
        public Dictionary<string, string> Labels { get; }
        public List<Tuple<double, long?>> Values { get; }

        public List<DataPoint> DataPoints { get; }

        public Metric(string name,DataPoint dataPoint)
        {
            Name = name;
            DataPoints=new List<DataPoint> { dataPoint };
        }

        public void AddDataPoint(DataPoint dataPoint)
        {
            DataPoints.Add(dataPoint);
        }
        public Metric(string name, Dictionary<string, string> labels, double value, long? timestamp = null)
        {
            Name = name;
            Labels = labels;
            Values = new List<Tuple<double, long?>>();
            Add(labels, value, timestamp);
        }

        public void Add(Dictionary<string, string> labels, double value, long? timestamp = null)
        {
            if (LabelsEqual(labels))
                Values.Add(new Tuple<double, long?>(value, timestamp));
        }

        private bool LabelsEqual(Dictionary<string, string> labels)
        {
            if (labels.Count != Labels.Count)
                return false;
            foreach (string key in labels.Keys)
            {
                if (!Labels.ContainsKey(key) || labels[key] != Labels[key])
                    return false;
            }
            return true;
        }
        public override string ToString()
        {
            IEnumerable<string> labelsWithValue = GetLabelsWithValue();           
            string str = string.Join('\n', labelsWithValue);
            return str.TrimEnd('\n');
        }

        private IEnumerable<string> GetLabelsWithValue()
        {
            List<string> labelsWithValue = new List<string>();
            foreach (DataPoint dataPoint in DataPoints)
            {
                List<string> labelsList = new List<string>();
                foreach(var lab in dataPoint.Labels)
                {
                    string label = string.Join("=", lab.Key,lab.Value);
                    labelsList.Add(label);
                }
                string labelWithNameAndValue;
                if (labelsList.Count > 0)
                {
                    string labels = string.Join(",", labelsList);
                    string enclosedLabel = "{" + labels + "}";
                    labelWithNameAndValue = string.Join(" ", Name, enclosedLabel, dataPoint.Value);
                }
                else
                {
                    labelWithNameAndValue = string.Join(" ", Name, dataPoint.Value);
                }
                               
                labelsWithValue.Add(labelWithNameAndValue);
            }
            return labelsWithValue;
        }
    }
}
