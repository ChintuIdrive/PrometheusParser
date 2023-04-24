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
            metricsParser.Parse();

            //Console.WriteLine("Hello, World!");
            //var endpoint = "play.min.io";
            //var accessKey = "tBxizjYNxL5JeFsf"; 
            //var secretKey = "w82tFI6im64grBXsG0vgmGkhIyszan9X";
            //var secure = true;

            //MinioClient minio = new MinioClient()
            //                        .WithEndpoint(endpoint)
            //                        .WithCredentials(accessKey, secretKey)
            //                        .WithSSL()
            //                        .Build();

            //// Create an async task for listing buckets.
            //var getListBucketsTask = await minio.ListBucketsAsync().ConfigureAwait(false);

            //// Iterate over the list of buckets.
            //foreach (var bucket in getListBucketsTask.Buckets)
            //{
            //    Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
            //}
        }

        //private static async Task m1(string[] args)
        //{
        //    var httpClient = new HttpClient();
        //    var metricsEndpoint = "https://localhost:8001/minio/v2/metrics/node";
        //    var response = await httpClient.GetAsync(metricsEndpoint);

        //    var parser = new MetricsParser(new DefaultMetricFactory());
            
        //    using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());

        //    while (!reader.EndOfStream)
        //    {
        //        var line = await reader.ReadLineAsync();
        //        parser.Parse(line);
        //    }

        //    var registry = new CollectorRegistry();
        //    registry.RegisterOnDemandParser(parser);
        //    var gauge = registry.GetSingleGauge("go_goroutines");
        //    Console.WriteLine(gauge.Value);
        //}
    }
}