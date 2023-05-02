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

            PrometheusBuilder builder = new PrometheusBuilder(new ParserHelper());
            PrometheusResponse response= builder.Build(lines);

        }
    }
}