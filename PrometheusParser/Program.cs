using Minio;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Prometheus;
using Minio.DataModel.Replication;
using Newtonsoft.Json;

namespace PrometheusParser
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //MetricsParser metricsParser = new MetricsParser();
            string PromResponseFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "PromResponses", "prom_response.txt");
            //metricsParser.GetRawMetrics(PromResponseFilePath);
            //metricsParser.Parse(PromResponseFilePath);

            //IDictionary<string, Metric> metrics = metricsParser.GetMetrics();


            // Print the metrics
            //foreach (Metric metric in metrics.Values)
            //{
            //    Console.WriteLine(metric.ToString());
            //}

            Parser parser = new Parser();
            parser.Load(PromResponseFilePath);

            IEnumerable<KeyValuePair<string, ISimpleMetric>> simpleMetrics = parser.GetSimpleMerics();
            foreach (var metric in simpleMetrics)
            {
                // Serialize the metric object to a JSON string
                string jsonString = JsonConvert.SerializeObject(metric, Formatting.Indented);

                // Print the JSON string to the console
                Console.WriteLine(jsonString);
                Console.WriteLine(metric.ToString());
            }

            IEnumerable<KeyValuePair<string, IComplexMetric>> complexMetrics = parser.GetComplexMerics();
            foreach (var metric in complexMetrics)
            {
                Console.WriteLine(metric.ToString());
            }
        }
    }
}