﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrometheusParser
{
    internal class PrometheusBuilder : IPrometheusBuilder
    {
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
            { "go_memstats_mallocs_total", (r, v) => r.go_memstats_mallocs_total = GetIntValue(v) },
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
            { "cluster_nodes_offline_total",(r,v )=> r.cluster_nodes_offline_total=GetIntValue(v) },
            { "cluster_nodes_online_total",(r,v) => r.cluster_nodes_online_total=GetIntValue(v)},
            { "process_cpu_seconds_total",(r,v)=>r.process_cpu_seconds_total=GetDoubleValue(v) },
            { "process_max_fds", (r, v) => r.process_max_fds = GetIntValue(v) },
            { "process_open_fds", (r, v) => r.process_open_fds = GetIntValue(v) },
            { "process_resident_memory_bytes", (r, v) => r.process_resident_memory_bytes = GetDoubleValue(v) },
            { "process_start_time_seconds", (r, v) => r.process_start_time_seconds = GetDoubleValue(v) },
            { "process_virtual_memory_bytes", (r, v) => r.process_virtual_memory_bytes = GetDoubleValue(v) },
            { "process_virtual_memory_max_bytes",(r,v)=>r.process_virtual_memory_max_bytes=GetDoubleValue(v) },
            { "s3_requests_incoming_total",(r,v)=>r.s3_requests_incoming_total=GetIntValue(v) },
            { "s3_requests_rejected_auth_total",(r,v)=>r.s3_requests_rejected_auth_total=GetIntValue(v) },
            { "s3_requests_rejected_header_total",(r,v)=>r.s3_requests_rejected_header_total=GetIntValue(v) },
            { "s3_requests_rejected_invalid_total",(r, v) => r.s3_requests_rejected_invalid_total=GetIntValue(v) },
            { "s3_requests_rejected_timestamp_total",(r,v)=>r.s3_requests_rejected_timestamp_total = GetIntValue(v) },
            { "s3_requests_waiting_total",(r,v)=>r.s3_requests_waiting_total = GetIntValue(v)},
            { "s3_traffic_received_bytes",(r,v)=>r.s3_traffic_received_bytes = GetDoubleValue(v) },
            { "s3_traffic_sent_bytes",(r,v)=>r.s3_traffic_sent_bytes=GetDoubleValue(v) }
        };
        private static readonly Dictionary<string, Action<PrometheusResponse, string, string>> labelMetricFieldMap = new Dictionary<string, Action<PrometheusResponse, string, string>>()
        {
             { "go_info", (r, l,v) =>
                 
                {
                    r.go_info.Add(l,GetDoubleValue(v));
                }
             },

             {
                "s3_requests_5xx_errors_total",(r,l,v)=>
                {
                    r.s3_requests_5xx_errors_total.Add(l,GetIntValue(v));
                }
             },

             {
                "software_commit_info",(r, l, v) =>
                {
                    r.software_commit_info.Add(l,GetIntValue(v));
                }
             },

             {
                "software_version_info",(r, l, v) =>
                {
                    r.software_version_info.Add(l,(GetIntValue(v)));
                }
             }
        };

        private static readonly Dictionary<string, string> _metricLabelMap = new Dictionary<string, string>()
        {
            { "go_info", "version"},
            { "s3_requests_5xx_errors_total", "api" },
            { "software_commit_info", "commit" },
            { "software_version_info", "version" }
        };

        private IParserHelper _parserHelper;

        private PrometheusResponse _prometheusResponse;
        public PrometheusBuilder(IParserHelper parserHelper)
        {
            _prometheusResponse = new PrometheusResponse(new ParserHelper());
            _parserHelper = parserHelper;
        }
        public PrometheusResponse Build(string[] lines)
        {//parse prometheus response
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
                    string metricLine = rawMetric[2];
                    string value = metricLine.Split(' ')[1];
                    string pattern = @"\{([^}]*)\}";
                    Match match = Regex.Match(metricLine, pattern);
                    if (match.Success)
                    {
                        string result = match.Groups[1].Value;
                        Dictionary<string,string> labelst = GetLabels(result);
                        if(labelst.Count() == 1 && labelst.ContainsKey("server"))
                        {
                            BuildSimpleMetric(name, value);
                        }
                        else
                        {
                            labelst.Remove("server");
                            string Label = GetLabel(name);
                            BuildSingleLabelMetric(name, Label,value);
                        }
                    }
                    else
                    {
                        BuildSimpleMetric(name, value);
                    }
                   
                }
                else if (rawMetric.Count() > 3)
                {
                    if (type == "summary")
                    {
                        //MapMetricWithMultiLabel(name, rawMetric.Skip(2).SkipLast(2).ToArray());

                        string sumMetricName = $"{name}_sum";
                        string sumMetricLine = rawMetric.Single(x => x.Contains(sumMetricName));
                        string sumValue = sumMetricLine.Split(' ')[1];
                        BuildSimpleMetric(sumMetricName, sumValue);

                        string countMetricName = $"{name}_count";
                        string countMetricLine = rawMetric.Single(x => x.Contains(countMetricName));
                        string countValue = countMetricLine.Split(" ")[1];
                        BuildSimpleMetric(countMetricName, countValue);

                    }
                    else
                    {
                        //MapMetricWithMultiLabel(name, rawMetric.Skip(2).ToArray());
                    }
                }
            }
            return _prometheusResponse;
        }
        private void BuildSimpleMetrics(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                switch (name)
                {
                    case "go_gc_duration_seconds_sum":
                        _prometheusResponse.go_gc_duration_seconds_sum = GetDoubleValue(value);
                        break;
                    case "go_gc_duration_seconds_count":
                        _prometheusResponse.go_gc_duration_seconds_count = GetIntValue(value);
                        break;
                    case "go_goroutines":
                        _prometheusResponse.go_goroutines = GetIntValue(value);
                        break;
                    case "go_memstats_alloc_bytes":
                        _prometheusResponse.go_memstats_alloc_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_alloc_bytes_total":
                        _prometheusResponse.go_memstats_alloc_bytes_total = GetDoubleValue(value);
                        break;
                    case "go_memstats_buck_hash_sys_bytes":
                        _prometheusResponse.go_memstats_buck_hash_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_frees_total":
                        _prometheusResponse.go_memstats_frees_total = GetDoubleValue(value);
                        break;
                    case "go_memstats_gc_sys_bytes":
                        _prometheusResponse.go_memstats_gc_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_heap_alloc_bytes":
                        _prometheusResponse.go_memstats_heap_alloc_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_heap_idle_bytes":
                        _prometheusResponse.go_memstats_heap_idle_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_heap_inuse_bytes":
                        _prometheusResponse.go_memstats_heap_inuse_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_heap_objects":
                        _prometheusResponse.go_memstats_heap_objects = GetIntValue(value);
                        break;
                    case "go_memstats_heap_released_bytes":
                        _prometheusResponse.go_memstats_heap_released_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_heap_sys_bytes":
                         _prometheusResponse.go_memstats_heap_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_last_gc_time_seconds":
                        _prometheusResponse.go_memstats_last_gc_time_seconds = GetDoubleValue(value);
                        break;
                    case "go_memstats_lookups_total":
                        _prometheusResponse.go_memstats_lookups_total = GetDoubleValue(value);
                        break;
                    case "go_memstats_mallocs_total":
                        _prometheusResponse.go_memstats_mallocs_total = GetDoubleValue(value);
                        break;
                    case "go_memstats_mcache_inuse_bytes":
                        _prometheusResponse.go_memstats_mcache_inuse_bytes = GetIntValue(value);
                        break;
                    case "go_memstats_mcache_sys_bytes":
                        _prometheusResponse.go_memstats_mcache_sys_bytes = GetIntValue(value);
                        break;
                    case "go_memstats_mspan_inuse_bytes":
                        _prometheusResponse.go_memstats_mspan_inuse_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_mspan_sys_bytes":
                        _prometheusResponse.go_memstats_mspan_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_next_gc_bytes":
                        _prometheusResponse.go_memstats_next_gc_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_other_sys_bytes":
                        _prometheusResponse.go_memstats_other_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_stack_inuse_bytes":
                        _prometheusResponse.go_memstats_stack_inuse_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_stack_sys_bytes":
                        _prometheusResponse.go_memstats_stack_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_memstats_sys_bytes":
                        _prometheusResponse.go_memstats_sys_bytes = GetDoubleValue(value);
                        break;
                    case "go_threads":
                        _prometheusResponse.go_threads = GetIntValue(value);
                        break;
                    case "cluster_nodes_offline_total":
                        _prometheusResponse.cluster_nodes_offline_total = GetIntValue(value);
                        break;
                    case "cluster_nodes_online_total":
                        _prometheusResponse.cluster_nodes_online_total = GetIntValue(value);
                        break;
                    case "process_cpu_seconds_total":
                        _prometheusResponse.process_cpu_seconds_total = GetDoubleValue(value);
                        break;
                    case "process_max_fds":
                        _prometheusResponse.process_max_fds = GetIntValue(value);
                        break;
                    case "process_open_fds":
                        _prometheusResponse.process_open_fds = GetIntValue(value);
                        break;
                    case "process_resident_memory_bytes":
                        _prometheusResponse.process_resident_memory_bytes = GetDoubleValue(value);
                        break;
                    case "process_start_time_seconds":
                        _prometheusResponse.process_start_time_seconds = GetDoubleValue(value);
                        break;
                    case "process_virtual_memory_bytes":
                        _prometheusResponse.process_virtual_memory_bytes = GetDoubleValue(value);
                        break;
                    case "process_virtual_memory_max_bytes":
                        _prometheusResponse.process_virtual_memory_max_bytes = GetDoubleValue(value);
                        break;
                    case "s3_requests_incoming_total":
                        _prometheusResponse.s3_requests_incoming_total = GetIntValue(value);
                        break;
                    case "s3_requests_rejected_auth_total":
                        _prometheusResponse.s3_requests_rejected_auth_total = GetIntValue(value);
                        break;
                    case "s3_requests_rejected_header_total":
                        _prometheusResponse.s3_requests_rejected_header_total = GetIntValue(value);
                        break;
                    case "s3_requests_rejected_invalid_total":
                        _prometheusResponse.s3_requests_rejected_invalid_total = GetIntValue(value);
                        break;
                    case "s3_requests_rejected_timestamp_total":
                        _prometheusResponse.s3_requests_rejected_timestamp_total = GetIntValue(value);
                        break;
                    case "s3_requests_waiting_total":
                        _prometheusResponse.s3_requests_waiting_total = GetIntValue(value);
                        break;
                    case "s3_traffic_received_bytes":
                        _prometheusResponse.s3_traffic_received_bytes = GetDoubleValue(value);
                        break;
                    case "s3_traffic_sent_bytes":
                        _prometheusResponse.s3_traffic_sent_bytes = GetDoubleValue(value);
                        break;
                    // more cases can be added here
                    default:
                        // Code to execute if expression doesn't match any case
                        break;
                }
            }
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
        private void BuildSingleLabelMetric(string name,string rawMetric)
        {
            string[] parts = rawMetric.Split(' ');
            string value = parts[1];

        }
        private void BuildMultiLabelMetric(string name, string[] rawMetrics)
        {

        }
        public PrometheusResponse Build()
        {
            throw new NotImplementedException();
        }
        bool IsSimpleMetric(string metricLine, out Dictionary<string, string> labels)
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
        private KeyValuePair<string, string> GetKeyValuePair(string metricLine, string key)
        {
            Dictionary<string, string> labels = GetLabels(metricLine);
            string valueString = metricLine.Split(" ")[1];
            string label = labels[key];
            return new KeyValuePair<string, string>(label, valueString);
        }
        private KeyValuePair<Tuple<string, double>, int> GetKeyValuePair(string metricLine, params string[] keys)
        {
            Dictionary<string, string> labels = GetLabels(metricLine);
            int value = GetIntValue(metricLine.Split(" ")[1].Trim());
            string apiLabel = labels[keys[0]];
            string le = labels[keys[1]].Trim('"');
            double leLabel = GetDoubleValue(le);

            return new KeyValuePair<Tuple<string, double>, int>(Tuple.Create(apiLabel, leLabel), value);
        }
        private Dictionary<string, string> GetLabels(string metricLine)
        {
            Dictionary<string, string> labels = new Dictionary<string, string>();
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
        private void InitAvgLoad()
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
        private string GetLabel(string name)
        {
            _metricLabelMap.TryGetValue(name, out var label);
            return label;
        }

    }
}