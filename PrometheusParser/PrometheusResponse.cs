using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Joins;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal class PrometheusResponse
    {
        public PrometheusResponse(string[] lines)
        {
            Init(lines);

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
        public Dictionary<double, double> go_gc_duration_seconds { get; private set; }

        public double go_gc_duration_seconds_sum ;
        public int go_gc_duration_seconds_count ;
        #endregion

        #region TYPE go_goroutines gauge
        /// <summary>
        /// # HELP go_goroutines Number of goroutines that currently exist.
        /// </summary>
        public int go_goroutines { get; private set; }
        #endregion

        public Dictionary<string,double> go_info { get; private set; }
        public double go_memstats_alloc_bytes { get; private set; }
        public double go_memstats_alloc_bytes_total { get; private set; }
        public double go_memstats_buck_hash_sys_bytes { get; private set; }
        public double go_memstats_frees_total { get; private set; }
        public double go_memstats_gc_sys_bytes { get; private set; }
        public double go_memstats_heap_alloc_bytes { get; private set; }
        public double go_memstats_heap_idle_bytes { get; private set; }
        public double go_memstats_heap_inuse_bytes { get; private set; }
        public int go_memstats_heap_objects { get; private set; }
        public double go_memstats_heap_released_bytes { get; private set;}
        public double go_memstats_heap_sys_bytes { get; private set; }
        public double go_memstats_last_gc_time_seconds { get; private set; }
        public double go_memstats_lookups_total { get; private set; }
        public double go_memstats_mallocs_total { get; private set; }
        public int go_memstats_mcache_inuse_bytes { get; private set; }
        public int go_memstats_mcache_sys_bytes { get; private set; }
        public double go_memstats_mspan_inuse_bytes { get; private set; }
        public double go_memstats_mspan_sys_bytes { get; private set; }
        public double go_memstats_next_gc_bytes { get; private set; }
        public double go_memstats_other_sys_bytes { get; private set; }
        public double go_memstats_stack_inuse_bytes { get; private set; }
        public double go_memstats_stack_sys_bytes { get; private set; }
        public double go_memstats_sys_bytes { get; private set; }
        public int go_threads { get; private set; }
        public int cluster_nodes_offline_total { get; private set; }
        public int cluster_nodes_online_total { get; private set; }
        public double process_cpu_seconds_total { get; private set; }
        public int process_max_fds { get; private set; }
        public int process_open_fds { get; private set; }
        public double process_resident_memory_bytes { get; private set; }
        public double process_start_time_seconds { get; private set; }
        public double process_virtual_memory_bytes { get; private set; }
        public double process_virtual_memory_max_bytes { get; private set; }
        public Dictionary<string, int> s3_requests_4xx_errors_total { get; private set; }
        public Dictionary<string, int> s3_requests_5xx_errors_total { get; private set; }
        public Dictionary<string, int> s3_requests_canceled_total { get; private set; }
        public Dictionary<string, int> s3_requests_errors_total { get; private set; }
        public int s3_requests_incoming_total { get; private set; }
        public Dictionary<string, int> s3_requests_inflight_total { get; private set; }
        public int s3_requests_rejected_auth_total { get; private set; }
        public int s3_requests_rejected_header_total { get; private set; }
        public int s3_requests_rejected_invalid_total { get; private set; }
        public int s3_requests_rejected_timestamp_total { get; private set; }
        public Dictionary<string, int> s3_requests_total { get; private set; }
        public int s3_requests_waiting_total { get; private set; }
        public Dictionary<Tuple<string,double>, int> s3_time_ttfb_seconds_distribution { get; private set; }
        public double s3_traffic_received_bytes { get; private set; }
        public double s3_traffic_sent_bytes { get; private set; }
        public Dictionary<string, int> software_commit_info { get; private set; }
        public Dictionary<string, int> software_version_info { get; private set; }
        private void Init(string[] lines)
        {
            Parser parser = new Parser();
            //parser.Load(promResponseFilePath);

            var rawMetrics = parser.GetRawMetrics(lines);
            foreach (var rawMetric in rawMetrics)
            {
                //split typeLine to extract name and type
                string typeLine = rawMetric[1];
                string[] parts = typeLine.Split(' ');
                string name = parts[2];
                string type = parts[3];
                if (rawMetric.Count() == 3)
                {
                    string metricLine= rawMetric[2];
                    string value = metricLine.Split(' ')[1];
                    if (IsSimpleMetric(metricLine, out Dictionary<string, string> labels))
                    {
                        MapSimpleMetric(name, value);
                    }
                    else
                    {
                        labels.Remove("server");
                        MapMetricWithLael(name, value,labels);
                    }
                }
                else if(rawMetric.Count() > 3)
                {
                    if (type == "summary")
                    {
                        MapMetricWithMultiLabel(name, rawMetric.Skip(2).SkipLast(2).ToArray());

                        string sumMetricName = $"{name}_sum";                       
                        string sumMetricLine = rawMetric.Single(x => x.Contains(sumMetricName));
                        string sumValue = sumMetricLine.Split(' ')[1];
                        MapSimpleMetric(sumMetricName, sumValue);

                        string countMetricName = $"{name}_count";
                        string countMetricLine = rawMetric.Single(x => x.Contains(countMetricName));
                        string countValue = countMetricLine.Split(" ")[1];
                        MapSimpleMetric(countMetricName, countValue);

                    }
                    else
                    {
                        MapMetricWithMultiLabel(name, rawMetric.Skip(2).ToArray());
                    }               
                }
            }
        }      
        private bool IsSimpleMetric(string metricLine, out Dictionary<string, string> labels)
        {
            labels = new Dictionary<string, string>();
            // Split the line into metric name, labels, and value
            string[] parts = metricLine.Split(' ');
            string value = parts[1];
            string pattern = @"\{([^}]*)\}";
            Match match = Regex.Match(metricLine, pattern);
            if (match.Success)
            {
                string result = match.Groups[1].Value;
                string[] rawlabels = result.Split(",");
                foreach (string label in rawlabels)
                {
                    string[] labelData = label.Split("=");
                    if (!labels.ContainsKey(label))
                    {
                        labels.Add(labelData[0], labelData[1]);
                    }
                }
                return labels.Count() == 1 && labels.ContainsKey("server");
            }
           return true;
         
        }
        private KeyValuePair<string,string> GetKeyValuePair(string metricLine,string key)
        {      
            Dictionary<string, string> labels = GetLabels(metricLine);
            string valueString = metricLine.Split(" ")[1];
            string label = labels[key];
            return new KeyValuePair<string, string>(label, valueString);
        }
        private KeyValuePair<Tuple<string,double>, int> GetKeyValuePair(string metricLine, params string[] keys)
        {
            Dictionary<string, string> labels = GetLabels(metricLine);
            int value = GetIntValue(metricLine.Split(" ")[1].Trim());
            string apiLabel = labels[keys[0]];
            string le = labels[keys[1]].Trim('"');
            double leLabel = GetDoubleValue(le);

            return new KeyValuePair<Tuple<string, double>, int>(Tuple.Create(apiLabel,leLabel), value);
        }
        private Dictionary<string, string> GetLabels(string metricLine)
        {
            Dictionary<string, string>  labels = new Dictionary<string, string>();           
            string[] parts = metricLine.Split(' ');
            string value = parts[1];
            string pattern = @"\{([^}]*)\}";
            Match match = Regex.Match(metricLine, pattern);
            if (match.Success)
            {
                string result = match.Groups[1].Value;
                string[] rawlabels = result.Split(",");

                foreach (string label in rawlabels)
                {
                    string[] labelData = label.Split("=");
                    if (!labels.ContainsKey(label))
                    {
                        labels.Add(labelData[0], labelData[1]);
                    }
                }
            }
            return labels;
        }
        void MapMetricWithLael(string name,string value, Dictionary<string, string> labels)
        {
            switch (name)
            {
                case "go_info":
                    go_info = new Dictionary<string, double>
                    {
                        { labels["version"], GetDoubleValue(value) }
                    };
                    break;
                case "s3_requests_5xx_errors_total":
                    string key = labels["api"];
                    int intValue = GetIntValue(value);
                    s3_requests_5xx_errors_total = new Dictionary<string, int>() 
                    { 
                        { key, intValue }
                    };
                    break;
                case "software_commit_info":
                    software_commit_info = new Dictionary<string, int>()
                    {
                        {labels["commit"], GetIntValue(value)}
                    };
                    break;
                case "software_version_info":
                    software_version_info = new Dictionary<string, int>()
                    {
                        { labels["version"], GetIntValue(value) }
                    };
                    break;
                default:
                    break;
            }
        }
        void MapMetricWithMultiLabel(string name, string[] rawMetrics)
        {         
            switch (name)
            {
                case "go_gc_duration_seconds":
                    go_gc_duration_seconds = new Dictionary<double, double>();
                    foreach (var rawMetric in rawMetrics)
                    {
                        KeyValuePair<string, string> keyValuePair = GetKeyValuePair(rawMetric, "quantile");
                        double key = GetDoubleValue(keyValuePair.Key.Trim('"'));
                        double value = GetDoubleValue(keyValuePair.Value);
                        if (!go_gc_duration_seconds.ContainsKey(key))
                        {
                            go_gc_duration_seconds.Add(key, value);
                        }
                    }
                    break;
                case "s3_requests_4xx_errors_total":
                    s3_requests_4xx_errors_total = new Dictionary<string, int>();
                    foreach (var rawMetric in rawMetrics)
                    {
                        KeyValuePair<string, string> keyValuePair = GetKeyValuePair(rawMetric,"api");

                        if (!s3_requests_4xx_errors_total.ContainsKey(keyValuePair.Key))
                        {
                            s3_requests_4xx_errors_total.Add(keyValuePair.Key,GetIntValue(keyValuePair.Value));
                        }                       
                    }
                    break;
                case "s3_requests_canceled_total":
                    s3_requests_canceled_total = new Dictionary<string, int> ();
                    foreach (var rawMetric in rawMetrics)
                    {
                        KeyValuePair<string, string> keyValuePair = GetKeyValuePair(rawMetric,"api");

                        if (!s3_requests_canceled_total.ContainsKey(keyValuePair.Key))
                        {
                            s3_requests_canceled_total.Add(keyValuePair.Key, GetIntValue(keyValuePair.Value));
                        }
                    }                   
                    break;
                case "s3_requests_errors_total":
                    s3_requests_errors_total = new Dictionary<string, int>();
                    foreach (var rawMetric in rawMetrics)
                    {
                        KeyValuePair<string, string> keyValuePair = GetKeyValuePair(rawMetric, "api");

                        if (!s3_requests_errors_total.ContainsKey(keyValuePair.Key))
                        {
                            s3_requests_errors_total.Add(keyValuePair.Key, GetIntValue(keyValuePair.Value));
                        }
                    }
                    break;
                case "s3_requests_inflight_total":
                    s3_requests_inflight_total = new Dictionary<string, int>();
                    foreach (var rawMetric in rawMetrics)
                    {
                        KeyValuePair<string, string> keyValuePair = GetKeyValuePair(rawMetric, "api");

                        if (!s3_requests_inflight_total.ContainsKey(keyValuePair.Key))
                        {
                            s3_requests_inflight_total.Add(keyValuePair.Key, GetIntValue(keyValuePair.Value));
                        }
                    }
                    break;
                case "s3_requests_total":
                    s3_requests_total=new Dictionary<string, int>();
                    foreach (var rawMetric in rawMetrics)
                    {
                        KeyValuePair<string, string> keyValuePair = GetKeyValuePair(rawMetric, "api");

                        if (!s3_requests_total.ContainsKey(keyValuePair.Key))
                        {
                            s3_requests_total.Add(keyValuePair.Key, GetIntValue(keyValuePair.Value));
                        }
                    }
                    break;
                case "s3_time_ttfb_seconds_distribution":
                    s3_time_ttfb_seconds_distribution = new Dictionary<Tuple<string, double>, int>();
                    foreach (var rawMetric in rawMetrics)
                    {
                        KeyValuePair<Tuple<string, double>, int> keyValuePair = GetKeyValuePair(rawMetric,"api", "le");
                        if (!s3_time_ttfb_seconds_distribution.ContainsKey(keyValuePair.Key))
                        {
                            s3_time_ttfb_seconds_distribution.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                    break;
                // more cases can be added here
                default:
                    // Code to execute if expression doesn't match any case
                    break;
            }
        }
        internal void MapSimpleMetric(string name, string value)
        {            
            if (!string.IsNullOrEmpty(value))
            {
                switch (name)
                {
                    case "go_gc_duration_seconds_sum":
                        go_gc_duration_seconds_sum=GetDoubleValue(value);
                        break;
                    case "go_gc_duration_seconds_count":
                        go_gc_duration_seconds_count=GetIntValue(value);
                        break;
                    case "go_goroutines":
                        go_goroutines = GetIntValue(value);                      
                        break;
                    case "go_memstats_alloc_bytes":
                        go_memstats_alloc_bytes = GetDoubleValue(value);                        
                        break;
                    case "go_memstats_alloc_bytes_total":
                        go_memstats_alloc_bytes_total= GetDoubleValue(value);
                        break;
                    case "go_memstats_buck_hash_sys_bytes":
                        go_memstats_buck_hash_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_frees_total":
                        go_memstats_frees_total= GetDoubleValue(value);
                        break;
                    case "go_memstats_gc_sys_bytes":
                        go_memstats_gc_sys_bytes= GetDoubleValue(value);
                        break;
                    case "go_memstats_heap_alloc_bytes":
                        go_memstats_heap_alloc_bytes= GetDoubleValue(value);
                        break;
                    case "go_memstats_heap_idle_bytes":
                        go_memstats_heap_idle_bytes= GetDoubleValue(value);
                        break;
                    case "go_memstats_heap_inuse_bytes":
                        go_memstats_heap_inuse_bytes= GetDoubleValue(value);
                        break;
                    case "go_memstats_heap_objects":
                        go_memstats_heap_objects= GetIntValue(value);
                        break;
                    case "go_memstats_heap_released_bytes":
                        go_memstats_heap_released_bytes= GetDoubleValue(value) ;
                        break;
                    case "go_memstats_heap_sys_bytes":
                        go_memstats_heap_sys_bytes=GetDoubleValue(value) ;
                        break;
                    case "go_memstats_last_gc_time_seconds":
                        go_memstats_last_gc_time_seconds= GetDoubleValue(value) ;
                        break;
                    case "go_memstats_lookups_total":
                        go_memstats_lookups_total= GetDoubleValue(value) ;
                        break;
                    case "go_memstats_mallocs_total":
                        go_memstats_mallocs_total= GetDoubleValue(value) ;
                        break;
                        case "go_memstats_mcache_inuse_bytes":
                        go_memstats_mcache_inuse_bytes=GetIntValue(value) ;
                        break;
                    case "go_memstats_mcache_sys_bytes":
                        go_memstats_mcache_sys_bytes=GetIntValue(value) ;
                        break;
                    case "go_memstats_mspan_inuse_bytes":
                        go_memstats_mspan_inuse_bytes=GetDoubleValue(value) ;
                        break;
                    case "go_memstats_mspan_sys_bytes":
                        go_memstats_mspan_sys_bytes= GetDoubleValue(value) ;
                        break;
                    case "go_memstats_next_gc_bytes":
                        go_memstats_next_gc_bytes = GetDoubleValue(value) ;
                        break;
                    case "go_memstats_other_sys_bytes":
                        go_memstats_other_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_stack_inuse_bytes":
                        go_memstats_stack_inuse_bytes= GetDoubleValue(value) ;
                        break;
                    case "go_memstats_stack_sys_bytes":
                        go_memstats_stack_sys_bytes=GetDoubleValue(value) ;
                        break;
                    case "go_memstats_sys_bytes":
                        go_memstats_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_threads":
                        go_threads=GetIntValue(value) ;
                        break;
                    case "cluster_nodes_offline_total":
                        cluster_nodes_offline_total=GetIntValue(value) ;
                        break;
                    case "cluster_nodes_online_total":
                        cluster_nodes_online_total=GetIntValue(value) ;
                        break;
                    case "process_cpu_seconds_total":
                        process_cpu_seconds_total=GetDoubleValue(value) ;
                        break;
                    case "process_max_fds":
                        process_max_fds=GetIntValue(value) ;
                        break;
                    case "process_open_fds":
                        process_open_fds=GetIntValue(value) ;
                        break;
                    case "process_resident_memory_bytes":
                        process_resident_memory_bytes = GetDoubleValue(value);
                        break;
                    case "process_start_time_seconds":
                        process_start_time_seconds=GetDoubleValue(value) ;
                        break;
                    case "process_virtual_memory_bytes":
                        process_virtual_memory_bytes=GetDoubleValue(value) ;
                        break;
                    case "process_virtual_memory_max_bytes":
                        process_virtual_memory_max_bytes=GetDoubleValue(value) ;
                        break;
                    case "s3_requests_incoming_total":
                        s3_requests_incoming_total=GetIntValue(value) ;
                        break;
                    case "s3_requests_rejected_auth_total":
                        s3_requests_rejected_auth_total=GetIntValue(value) ;
                        break;
                    case "s3_requests_rejected_header_total":
                        s3_requests_rejected_header_total=GetIntValue(value) ;
                        break;
                    case "s3_requests_rejected_invalid_total":
                        s3_requests_rejected_invalid_total=GetIntValue(value) ;
                        break;
                    case "s3_requests_rejected_timestamp_total":
                        s3_requests_rejected_timestamp_total=GetIntValue(value) ;
                        break;
                    case "s3_requests_waiting_total":
                        s3_requests_waiting_total=GetIntValue(value) ;
                        break;
                    case "s3_traffic_received_bytes":
                        s3_traffic_received_bytes=GetDoubleValue(value) ;
                        break;
                    case "s3_traffic_sent_bytes":
                        s3_traffic_sent_bytes=GetDoubleValue(value) ;
                        break;
                    // more cases can be added here
                    default:
                        // Code to execute if expression doesn't match any case
                        break;
                }
            }
            
        }
        private double GetDoubleValue(string value)
        {
            if (double.TryParse(value, out double doubleValue))
            {
                return doubleValue;
            }
            return double.MinValue;
        }
        private int GetIntValue(string value)
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }
            return int.MinValue;
        }
    }
}
