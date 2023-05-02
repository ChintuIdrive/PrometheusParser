using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PrometheusParser
{
    public class PrometheusBuilder : IPrometheusBuilder
    {
        private const string labelPattern = @"\{([^}]*)\}";
        private static readonly Dictionary<string, Action<PrometheusResponse, string>> metricFieldMap = new Dictionary<string, Action<PrometheusResponse, string>>
        {
            { "go_gc_duration_seconds_sum", (r, v) => r.go_gc_duration_seconds_sum = GetDoubleValue(v) },
            { "go_gc_duration_seconds_count", (r, v) => r.go_gc_duration_seconds_count = GetIntValue(v) },
            { "go_goroutines", (r, v) => r.go_goroutines = GetIntValue(v) },
            { "go_memstats_alloc_bytes", (r, v) => r.go_memstats_alloc_bytes = GetDoubleValue(v) },
            { "go_memstats_alloc_bytes_total", (r, v) => r.go_memstats_alloc_bytes_total = GetDoubleValue(v) },
            { "go_memstats_buck_hash_sys_bytes", (r, v) => r.go_memstats_buck_hash_sys_bytes = GetDoubleValue(v) },
            { "go_memstats_frees_total", (r, v) => r.go_memstats_frees_total = GetDoubleValue(v) },
            { "go_memstats_gc_sys_bytes", (r, v) => r.go_memstats_gc_sys_bytes = GetDoubleValue(v) },
            { "go_memstats_heap_alloc_bytes", (r, v) => r.go_memstats_heap_alloc_bytes = GetDoubleValue(v) },
            { "go_memstats_heap_idle_bytes", (r, v) => r.go_memstats_heap_idle_bytes = GetDoubleValue(v) },
            { "go_memstats_heap_inuse_bytes", (r, v) => r.go_memstats_heap_inuse_bytes = GetDoubleValue(v) },
            { "go_memstats_heap_objects", (r, v) => r.go_memstats_heap_objects = GetIntValue(v) },
            { "go_memstats_heap_released_bytes", (r, v) => r.go_memstats_heap_released_bytes = GetDoubleValue(v) },
            { "go_memstats_heap_sys_bytes", (r, v) => r.go_memstats_heap_sys_bytes = GetDoubleValue(v) },
            { "go_memstats_last_gc_time_seconds", (r, v) => r.go_memstats_last_gc_time_seconds = GetDoubleValue(v) },
            { "go_memstats_lookups_total", (r, v) => r.go_memstats_lookups_total = GetDoubleValue(v) },
            { "go_memstats_mallocs_total", (r, v) => r.go_memstats_mallocs_total = GetDoubleValue(v) },
            { "go_memstats_mcache_inuse_bytes", (r, v) => r.go_memstats_mcache_inuse_bytes = GetIntValue(v) },
            { "go_memstats_mcache_sys_bytes", (r, v) => r.go_memstats_mcache_sys_bytes = GetIntValue(v) },
            { "go_memstats_mspan_inuse_bytes", (r, v) => r.go_memstats_mspan_inuse_bytes = GetDoubleValue(v) },
            { "go_memstats_mspan_sys_bytes", (r, v) => r.go_memstats_mspan_sys_bytes = GetDoubleValue(v) },
            { "go_memstats_next_gc_bytes", (r, v) => r.go_memstats_next_gc_bytes = GetDoubleValue(v) },
            { "go_memstats_other_sys_bytes", (r, v) => r.go_memstats_other_sys_bytes = GetDoubleValue(v) },
            { "go_memstats_stack_inuse_bytes", (r, v) => r.go_memstats_stack_inuse_bytes = GetDoubleValue(v) },
            { "go_memstats_stack_sys_bytes", (r, v) => r.go_memstats_stack_sys_bytes = GetDoubleValue(v) },
            { "go_memstats_sys_bytes", (r, v) => r.go_memstats_sys_bytes = GetDoubleValue(v) },
            { "go_threads", (r, v) => r.go_threads = GetIntValue(v) },
            { "minio_cluster_nodes_offline_total",(r,v )=> r.cluster_nodes_offline_total=GetIntValue(v) },
            { "minio_cluster_nodes_online_total",(r,v) => r.cluster_nodes_online_total=GetIntValue(v)},
            { "minio_process_cpu_seconds_total",(r,v)=>r.process_cpu_seconds_total=GetDoubleValue(v) },
            { "minio_process_max_fds", (r, v) => r.process_max_fds = GetIntValue(v) },
            { "minio_process_open_fds", (r, v) => r.process_open_fds = GetIntValue(v) },
            { "minio_process_resident_memory_bytes", (r, v) => r.process_resident_memory_bytes = GetDoubleValue(v) },
            { "minio_process_start_time_seconds", (r, v) => r.process_start_time_seconds = GetDoubleValue(v) },
            { "minio_process_virtual_memory_bytes", (r, v) => r.process_virtual_memory_bytes = GetDoubleValue(v) },
            { "minio_process_virtual_memory_max_bytes",(r,v)=>r.process_virtual_memory_max_bytes=GetDoubleValue(v) },
            { "minio_s3_requests_incoming_total",(r,v)=>r.s3_requests_incoming_total=GetIntValue(v) },
            { "minio_s3_requests_rejected_auth_total",(r,v)=>r.s3_requests_rejected_auth_total=GetIntValue(v) },
            { "minio_s3_requests_rejected_header_total",(r,v)=>r.s3_requests_rejected_header_total=GetIntValue(v) },
            { "minio_s3_requests_rejected_invalid_total",(r, v) => r.s3_requests_rejected_invalid_total=GetIntValue(v) },
            { "minio_s3_requests_rejected_timestamp_total",(r,v)=>r.s3_requests_rejected_timestamp_total = GetIntValue(v) },
            { "minio_s3_requests_waiting_total",(r,v)=>r.s3_requests_waiting_total = GetIntValue(v)},
            { "minio_s3_traffic_received_bytes",(r,v)=>r.s3_traffic_received_bytes = GetDoubleValue(v) },
            { "minio_s3_traffic_sent_bytes",(r,v)=>r.s3_traffic_sent_bytes=GetDoubleValue(v) }
        };
        private static readonly Dictionary<string, Action<PrometheusResponse, string, string>> labelMetricFieldMap = new Dictionary<string, Action<PrometheusResponse, string, string>>()
        {
             { "go_info", (r, l,v) =>
                 
                {
                    r.go_info.Add(l,GetIntValue(v));
                }
             },

             {
                "minio_s3_requests_5xx_errors_total",(r,l,v)=>
                {
                    r.s3_requests_5xx_errors_total.Add(l,GetIntValue(v));
                }
             },

             {
                "minio_software_commit_info",(r, l, v) =>
                {
                    r.software_commit_info.Add(l,GetIntValue(v));
                }
             },

             {
                "minio_software_version_info",(r, l, v) =>
                {
                    r.software_version_info.Add(l,GetIntValue(v));
                }
             },

             {
                "go_gc_duration_seconds",
                (r, l, v) =>
                {
                    double labelValue= GetDoubleValue(l);
                    if (!r.go_gc_duration_seconds.ContainsKey(labelValue))
                    {
                        r.go_gc_duration_seconds.Add(labelValue, GetDoubleValue(v));
                    }
                }
             },

            {
                "minio_s3_requests_4xx_errors_total",
                (r, l, v) =>
                {
                    if (!r.s3_requests_4xx_errors_total.ContainsKey(l))
                    {
                        r.s3_requests_4xx_errors_total.Add(l, GetIntValue(v));
                    }
                }
            },

            {
                "minio_s3_requests_canceled_total",
                (r, l, v) =>
                {
                    if (!r.s3_requests_canceled_total.ContainsKey(l))
                    {
                        r.s3_requests_canceled_total.Add(l, GetIntValue(v));
                    }
                }
            },

            {
                "minio_s3_requests_errors_total",
                (r, l, v) =>
                {
                    if (!r.s3_requests_errors_total.ContainsKey(l))
                    {
                        r.s3_requests_errors_total.Add(l, GetIntValue(v));
                    }
                }
            },

            {
                "minio_s3_requests_inflight_total",
                (r, l, v) =>
                {
                    if (!r.s3_requests_inflight_total.ContainsKey(l))
                    {
                        r.s3_requests_inflight_total.Add(l, GetIntValue(v));
                    }
                }
            },

            {
                "minio_s3_requests_total",
                (r, l, v) =>
                {
                    if (!r.s3_requests_total.ContainsKey(l))
                    {
                        r.s3_requests_total.Add(l, GetIntValue(v));
                    }
                }
            },

             {
                "minio_s3_time_ttfb_seconds_distribution",
                (r, l, v) =>
                {
                    if (!r.s3_time_ttfb_seconds_distribution.ContainsKey(l))
                    {
                        r.s3_time_ttfb_seconds_distribution.Add(l, GetIntValue(v));
                    }
                }
            },
        };

        private static readonly Dictionary<string, string> _metricLabelMap = new Dictionary<string, string>()
        {
            { "go_info", "version"},
            { "minio_software_commit_info", "commit" },
            { "minio_software_version_info", "version" },
            { "go_gc_duration_seconds", "quantile"},
            { "minio_s3_requests_4xx_errors_total","api"},
            { "minio_s3_requests_5xx_errors_total", "api" },
            { "minio_s3_requests_canceled_total","api"},
            { "minio_s3_requests_errors_total","api"},
            { "minio_s3_requests_inflight_total","api" },
            { "minio_s3_requests_total","api"},        

        };
        private static readonly Dictionary<string, string[]> _metricMultiLabelMap = new Dictionary<string, string[]>()
        {
               { "minio_s3_time_ttfb_seconds_distribution",new string[]{ "api" ,"le"} }
        };

        private IParserHelper _parserHelper;

        private PrometheusResponse _prometheusResponse;
        public PrometheusBuilder(IParserHelper parserHelper)
        {
            _prometheusResponse = new PrometheusResponse();
            _parserHelper = parserHelper;
        }
        public PrometheusResponse Build(string[] lines)
        {
            //parse prometheus response
            IEnumerable<string[]> rawMetrics = _parserHelper.GetRawMetrics(lines);
            foreach (var rawMetric in rawMetrics)
            {
                //split typeLine to extract name and type
                string typeLine = rawMetric[1];
                string[] parts = typeLine.Split(' ');
                string name = parts[2];
                string type = parts[3];
                if (rawMetric.Count() == 3)
                {
                    BuildSimpleAndSingleMetric(rawMetric, name);

                }
                else if (rawMetric.Count() > 3)
                {
                    if (type == "summary")
                    {
                        BuildSummaryMetric(name, rawMetric);
                    }
                    else
                    {
                        BuildMultiLabelMetric(name, rawMetric.Skip(2).ToArray());
                    }
                }
            }
            BuildAvgLoad();
            return _prometheusResponse;
        }
        private void BuildSimpleAndSingleMetric(string[] rawMetric, string name)
        {
            string metricLine = rawMetric[2];
            string value = metricLine.Split(' ')[1];
            Match match = Regex.Match(metricLine, labelPattern);
            if (match.Success)
            {
                string result = match.Groups[1].Value;
                Dictionary<string, string> labels = GetLabels(result);
                if (labels.Count() == 1 && labels.ContainsKey("server"))
                {
                    BuildSimpleMetric(name, value);
                }
                else
                {
                    labels.Remove("server");
                    string label = _metricLabelMap[name];
                    string labelValue = labels[label];
                    BuildSingleLabelMetric(name, labelValue, value);
                }
            }
            else
            {
                BuildSimpleMetric(name, value);
            }
        }
        private void BuildSummaryMetric(string name,string[] rawMetrics)
        {
            BuildMultiLabelMetric(name, rawMetrics.Skip(2).SkipLast(2).ToArray());

            string sumMetricName = $"{name}_sum";
            string sumMetricLine = rawMetrics.Single(x => x.Contains(sumMetricName));
            string sumValue = sumMetricLine.Split(' ')[1];
            BuildSimpleMetric(sumMetricName, sumValue);

            string countMetricName = $"{name}_count";
            string countMetricLine = rawMetrics.Single(x => x.Contains(countMetricName));
            string countValue = countMetricLine.Split(" ")[1];
            BuildSimpleMetric(countMetricName, countValue);
        }
        private void BuildSimpleMetric(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (metricFieldMap.TryGetValue(name, out var updateAction))
            {
                updateAction(_prometheusResponse, value);
            }
        }
        private void BuildSingleLabelMetric(string name,string label, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (labelMetricFieldMap.TryGetValue(name, out var updateAction))
            {
                updateAction(_prometheusResponse,label, value);
            }
        }
        private void BuildMultiLabelMetric(string name, string[] rawMetrics)
        {
            foreach (string metric in rawMetrics)
            {
                string[] parts = metric.Split(" ");
                Match match = Regex.Match(parts[0], labelPattern);
                if (match.Success)
                {
                    string result = match.Groups[1].Value;
                    Dictionary<string, string> labels = GetLabels(result);
                    labels.Remove("server");
                    if (labels.Count > 1)
                    {
                        string[] labelsKeys = _metricMultiLabelMap[name];
                        List<string> labelValues = new List<string>();
                        foreach(string key in labelsKeys)
                        {
                            labelValues.Add(labels[key]);
                        }
                        string labelValue=string.Join(":", labelValues);
                        BuildSingleLabelMetric(name, labelValue, parts[1]);
                    }
                    else
                    {
                        string label = _metricLabelMap[name];
                        string labelValue = labels[label];
                        BuildSingleLabelMetric(name, labelValue, parts[1]);
                    }
                    
                }
            }
        }
        private Dictionary<string, string> GetLabels(string result)
        {
            Dictionary<string, string> labels = new Dictionary<string, string>();  
            
            string[] rawlabels = result.Split(",").Select(s => s.Trim('"')).ToArray();
            foreach (string label in rawlabels)
            {
                string[] labelData = label.Split("=").Select(s => s.Trim('"')).ToArray();
                if (!labels.ContainsKey(label))
                {
                    labels.Add(labelData[0], labelData[1]);
                }
            }
            return labels;
        }
        private static double GetDoubleValue(string value)
        {
            if (double.TryParse(value, out double doubleValue))
            {
                return doubleValue;
            }
            return double.MinValue;
        }
        private static int GetIntValue(string value)
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }
            return int.MinValue;
        }
        private void BuildAvgLoad()
        {
            // Get the current system load average 
            string averageLoadCommand = "uptime | awk -F 'load average: ' '{print $2}'";
            // create a new process
            Process process = new Process();
            // specify the bash executable as the process to start
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{averageLoadCommand}\"";
            //process.StartInfo.UseShellExecute = false;
            //process.StartInfo.RedirectStandardOutput = true;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            //process.StartInfo.WorkingDirectory = "/home/chintu/BatchesIn10K/";

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Command failed with exit code {process.ExitCode}: {averageLoadCommand}");
            }
            string output = process.StandardOutput.ReadToEnd();
            string[] avgLoads = output.Trim().Split(',');
            _prometheusResponse.avgLoad1 = GetDoubleValue(avgLoads[0].Trim());
            _prometheusResponse.avgLoad5 = GetDoubleValue(avgLoads[1].Trim());
            _prometheusResponse.avgLoad15 = GetDoubleValue(avgLoads[2].Trim());

        }      

    }
}
