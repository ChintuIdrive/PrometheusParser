namespace PrometheusParser
{
    public interface IParserHelper
    {
        IEnumerable<string[]> GetRawMetrics(string[] lines);
    }
}