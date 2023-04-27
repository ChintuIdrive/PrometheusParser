using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal class PrometheusResponse
    {
        public PrometheusResponse(string promResponseFilePath)
        {
            Init(promResponseFilePath);

        }
        #region avgLoad
        int avgLoad5;
        int avgLoad10;
        int avgLoad15;
        #endregion

        #region TYPE go_gc_duration_seconds summary
        /// <summary>
        /// HELP go_gc_duration_seconds A summary of the pause duration of garbage collection cycles.
        /// </summary>
        public Dictionary<double, double> go_gc_duration_seconds { get; }

        public double go_gc_duration_seconds_sum ;
        public int go_gc_duration_seconds_count ;
        #endregion

        #region TYPE go_goroutines gauge
        /// <summary>
        /// # HELP go_goroutines Number of goroutines that currently exist.
        /// </summary>
        public int go_goroutines;
        #endregion

        public Dictionary<string,float> go_info { get; }
        public double go_memstats_alloc_bytes { get; }
        public double go_memstats_alloc_bytes_total { get; }
        public double go_memstats_buck_hash_sys_bytes { get; }
        public double go_memstats_frees_total { get; }
        public double go_memstats_gc_sys_bytes { get; }
        public double go_memstats_heap_alloc_bytes { get; }
        public double go_memstats_heap_idle_bytes { get; }
        public double go_memstats_heap_inuse_bytes { get; }
        public int go_memstats_heap_objects { get; }
        public double go_memstats_heap_released_bytes { get; }
        public double go_memstats_heap_sys_bytes { get; }
        public double go_memstats_last_gc_time_seconds { get; }
        public double go_memstats_lookups_total { get; }
        public double go_memstats_mallocs_total { get; }
        public int go_memstats_mcache_inuse_bytes { get; }
        public int go_memstats_mcache_sys_bytes { get; }
        public double go_memstats_mspan_inuse_bytes { get; }
        public double go_memstats_mspan_sys_bytes { get; }
        public double go_memstats_next_gc_bytes { get; }
        public double go_memstats_other_sys_bytes { get; }
        public double go_memstats_stack_inuse_bytes { get; }
        public double go_memstats_stack_sys_bytes { get; }
        public double go_memstats_sys_bytes { get; }
        public int go_threads { get; }
        public int minio_cluster_nodes_offline_total { get; }
        public int minio_cluster_nodes_online_total { get; }
        public double minio_process_cpu_seconds_total { get; }
        public int minio_process_max_fds { get; }
        public int minio_process_open_fds { get; }
        public double minio_process_resident_memory_bytes { get; }
        public double minio_process_start_time_seconds { get; }
        public double minio_process_virtual_memory_bytes { get; }
        public double minio_process_virtual_memory_max_bytes { get; }

        public Dictionary<string, int> minio_s3_requests_4xx_errors_total { get; }
        public Dictionary<string, int> minio_s3_requests_5xx_errors_total { get; }
        public Dictionary<string, int> minio_s3_requests_canceled_total { get; }
        public Dictionary<string, int> minio_s3_requests_errors_total { get; }
        public int minio_s3_requests_incoming_total { get; }
        public Dictionary<string, int> minio_s3_requests_inflight_total { get; }
        public int minio_s3_requests_rejected_auth_total { get; }
        public int minio_s3_requests_rejected_header_total { get; }
        public int minio_s3_requests_rejected_invalid_total { get; }
        public int minio_s3_requests_rejected_timestamp_total { get; }
        public Dictionary<string, int> minio_s3_requests_total { get; }
        public int minio_s3_requests_waiting_total { get; }
        public Dictionary<string, int> minio_s3_time_ttfb_seconds_distribution { get; }
        public double minio_s3_traffic_received_bytes { get; }
        public double minio_s3_traffic_sent_bytes { get; }
        public Dictionary<string, int> minio_software_commit_info { get; }
        public Dictionary<string, int> minio_software_version_info { get; }

        private void Init(string promResponseFilePath)
        {
            Parser parser = new Parser();
            parser.Load(promResponseFilePath);
            IEnumerable<KeyValuePair<string, ISimpleMetric>> simpleMetrics = parser.GetSimpleMerics();
            MapSimpleType(simpleMetrics);


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
        internal void MapSimpleType(IEnumerable<KeyValuePair<string, ISimpleMetric>> simpleMetrics)
        {
            foreach (var simpleMetric in simpleMetrics)
            {
                switch (simpleMetric.Key)
                {
                    case "go_goroutines":
                        foreach (DataPoint dataPoint in simpleMetric.Value.DataPoints)
                        {
                            foreach (var labelWithValue in dataPoint.Labels)
                            {
                                
                            }
                        }
                        break;
                    case "go_info":
                        // Code to execute if expression equals value2
                        break;
                    // more cases can be added here
                    default:
                        // Code to execute if expression doesn't match any case
                        break;
                }
            }
        }
    }
}
