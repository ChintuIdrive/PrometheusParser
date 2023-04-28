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
            string PromResponseFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "PromResponses", "prom_response.txt");
           
            string[] lines = File.ReadAllLines(PromResponseFilePath);
            PrometheusResponse prometheusResponse = new PrometheusResponse(lines);
            foreach(var a in prometheusResponse.s3_requests_inflight_total)
            {
                Console.WriteLine(a);
            }
            
        }
    }
}