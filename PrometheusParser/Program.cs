using Minio;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Prometheus;
using Minio.DataModel.Replication;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

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
            string[] lines = File.ReadAllLines(PromResponseFilePath);
            PrometheusResponse prometheusResponse = new PrometheusResponse(lines);
            foreach(var a in prometheusResponse.minio_s3_requests_inflight_total)
            {
                Console.WriteLine(a);
            }
            
        }
    }
}