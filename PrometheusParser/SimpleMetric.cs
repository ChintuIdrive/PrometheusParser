using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal class SimpleMetric : ISimpleMetric
    {   
        public string Name { get; }
        public string Type { get; }
        public string Description { get; }
        public List<DataPoint> DataPoints {get;}
        public SimpleMetric(string name,string type,string description, DataPoint dataPoint)
        {
            Name = name;
            Type = type;
            Description = description;
            DataPoints = new List<DataPoint> { dataPoint };
        }
        public void AddDataPoint(DataPoint dataPoint)
        {
            DataPoints.Add(dataPoint);
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
                foreach (var lab in dataPoint.Labels)
                {
                    string label = string.Join("=", lab.Key, lab.Value);
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
