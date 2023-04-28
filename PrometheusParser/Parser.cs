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
        public List<string[]> GetRawMetrics(string[] lines)
        {
            List<string[]> rawMetrics = new List<string[]>();
            // Read the file and split the response into lines         
            lines = lines.Skip(1).ToArray();

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
    }
}
