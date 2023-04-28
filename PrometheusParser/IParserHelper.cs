namespace PrometheusParser
{
    internal interface IParserHelper
    {
        IEnumerable<string[]> GetRawMetrics(string[] lines);
    }
}