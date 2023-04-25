using Minio;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Prometheus;
using Minio.DataModel.Replication;

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

            IDictionary<string, ISimpleMetric> simpleMetrics = parser.GetSimpleMerics();
            foreach (var metric in simpleMetrics)
            {
                Console.WriteLine(metric.ToString());
            }

            IDictionary<string, IComplexMetric> complexMetrics = parser.GetComplexMerics();
            foreach (var metric in complexMetrics)
            {
                Console.WriteLine(metric.ToString());
            }
        }
    }
}