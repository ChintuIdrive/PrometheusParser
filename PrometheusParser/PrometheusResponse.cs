using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Joins;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrometheusParser
{
    public class PrometheusResponse
    {
        public PrometheusResponse()
        {
            go_gc_duration_seconds = new Dictionary<double, double>();
            go_info = new Dictionary<string, int>();
            s3_requests_4xx_errors_total = new Dictionary<string, int>();
            s3_requests_5xx_errors_total = new Dictionary<string, int>();
            s3_requests_canceled_total = new Dictionary<string, int>();
            s3_requests_errors_total = new Dictionary<string, int>();
            s3_requests_inflight_total = new Dictionary<string, int>();
            s3_requests_total = new Dictionary<string, int>();
            s3_time_ttfb_seconds_distribution = new Dictionary<string, int>();
            software_commit_info = new Dictionary<string, int>();
            software_version_info = new Dictionary<string, int>();
        }
        #region avgLoad
        public double avgLoad1 { get; internal set; }
        public double avgLoad5 { get; internal set; }
        public double avgLoad15 { get; internal set; }
        #endregion

        #region TYPE go_gc_duration_seconds summary
        /// <summary>
        /// HELP go_gc_duration_seconds A summary of the pause duration of garbage collection cycles.
        /// </summary>
        public Dictionary<double, double> go_gc_duration_seconds { get; internal set; }

        public double go_gc_duration_seconds_sum;
        public int go_gc_duration_seconds_count;
        #endregion

        #region TYPE go_goroutines gauge
        /// <summary>
        /// # HELP go_goroutines Number of goroutines that currently exist.
        /// </summary>
        public int go_goroutines { get; internal set; }
        #endregion

        public Dictionary<string, int> go_info { get; internal set; }
        public double go_memstats_alloc_bytes { get; internal set; }
        public double go_memstats_alloc_bytes_total { get; internal set; }
        public double go_memstats_buck_hash_sys_bytes { get; internal set; }
        public double go_memstats_frees_total { get; internal set; }
        public double go_memstats_gc_sys_bytes { get; internal set; }
        public double go_memstats_heap_alloc_bytes { get; internal set; }
        public double go_memstats_heap_idle_bytes { get; internal set; }
        public double go_memstats_heap_inuse_bytes { get; internal set; }
        public int go_memstats_heap_objects { get; internal set; }
        public double go_memstats_heap_released_bytes { get; internal set; }
        public double go_memstats_heap_sys_bytes { get; internal set; }
        public double go_memstats_last_gc_time_seconds { get; internal set; }
        public double go_memstats_lookups_total { get; internal set; }
        public double go_memstats_mallocs_total { get; internal set; }
        public int go_memstats_mcache_inuse_bytes { get; internal set; }
        public int go_memstats_mcache_sys_bytes { get; internal set; }
        public double go_memstats_mspan_inuse_bytes { get; internal set; }
        public double go_memstats_mspan_sys_bytes { get; internal set; }
        public double go_memstats_next_gc_bytes { get; internal set; }
        public double go_memstats_other_sys_bytes { get; internal set; }
        public double go_memstats_stack_inuse_bytes { get; internal set; }
        public double go_memstats_stack_sys_bytes { get; internal set; }
        public double go_memstats_sys_bytes { get; internal set; }
        public int go_threads { get; internal set; }
        public int cluster_nodes_offline_total { get; internal set; }
        public int cluster_nodes_online_total { get; internal set; }
        public double process_cpu_seconds_total { get; internal set; }
        public int process_max_fds { get; internal set; }
        public int process_open_fds { get; internal set; }
        public double process_resident_memory_bytes { get; internal set; }
        public double process_start_time_seconds { get; internal set; }
        public double process_virtual_memory_bytes { get; internal set; }
        public double process_virtual_memory_max_bytes { get; internal set; }
        public Dictionary<string, int> s3_requests_4xx_errors_total { get; internal set; }
        public Dictionary<string, int> s3_requests_5xx_errors_total { get; internal set; }
        public Dictionary<string, int> s3_requests_canceled_total { get; internal set; }
        public Dictionary<string, int> s3_requests_errors_total { get; internal set; }
        public int s3_requests_incoming_total { get; internal set; }
        public Dictionary<string, int> s3_requests_inflight_total { get; internal set; }
        public int s3_requests_rejected_auth_total { get; internal set; }
        public int s3_requests_rejected_header_total { get; internal set; }
        public int s3_requests_rejected_invalid_total { get; internal set; }
        public int s3_requests_rejected_timestamp_total { get; internal set; }
        public Dictionary<string, int> s3_requests_total { get; internal set; }
        public int s3_requests_waiting_total { get; internal set; }
        public Dictionary<string, int> s3_time_ttfb_seconds_distribution { get; internal set; }
        public double s3_traffic_received_bytes { get; internal set; }
        public double s3_traffic_sent_bytes { get; internal set; }
        public Dictionary<string, int> software_commit_info { get; internal set; }
        public Dictionary<string, int> software_version_info { get; internal set; }
    }
}
