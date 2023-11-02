using WebScrapingBenchmark.Framework.UrlScrapingResults;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults
{
    public class ElementTiming
    {
        public string SelectorName;
        public TimeSpan Duration;
    }

    public static class TimeSpanExtension
    {
        public static TimeSpan Average(this IEnumerable<TimeSpan> spans)
        {
            if (spans == null || !spans.Any()) return TimeSpan.Zero;

            return TimeSpan.FromSeconds(spans.Select(s => s.TotalSeconds).Average());
        }
    }
}
