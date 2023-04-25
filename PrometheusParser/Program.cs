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
            MetricsParser metricsParser = new MetricsParser();
            string PromResponseFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "PromResponses", "prom_response.txt");
            metricsParser.Parse(PromResponseFilePath);

            IDictionary<string, Metric> metrics = metricsParser.GetMetrics();

            // Print the metrics
            foreach (Metric metric in metrics.Values)
            {
                Console.WriteLine(metric.ToString());
            }

        }
    }
}