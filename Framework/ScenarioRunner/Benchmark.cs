namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class Benchmark
    {
        public string ScenarioName;
        public string ScraperName;

        public List<ScrapingBenchmarkResult> BenchmarkPerUrl = new();

        public Lazy<TimeSpan> AverageGoToUrl => new Lazy<TimeSpan>(() => BenchmarkPerUrl.Select(url => url.GoToUrlTiming).Average());
        public Lazy<TimeSpan> AverageLoad => new Lazy<TimeSpan>(() => BenchmarkPerUrl.Select(url => url.LoadTiming).Average());
        public Lazy<TimeSpan> AverageGetHtmlResult => new Lazy<TimeSpan>(() => BenchmarkPerUrl.Select(url => url.GetHtmlResultTiming).Average());
        public Lazy<TimeSpan> AverageMetadataExtraction => new Lazy<TimeSpan>(() => BenchmarkPerUrl.SelectMany(url => url.MetadataExtractionTiming.Select(timing => timing.Duration)).Average());
        public Lazy<TimeSpan> AverageContentExclusion => new Lazy<TimeSpan>(() => BenchmarkPerUrl.SelectMany(url => url.ContentExclusionTiming.Select(timing => timing.Duration)).Average());

        public Lazy<TimeSpan> FastestGoToUrl => new Lazy<TimeSpan>(() => BenchmarkPerUrl.Select(url => url.GoToUrlTiming).Min());
        public Lazy<TimeSpan> FastestLoad => new Lazy<TimeSpan>(() => BenchmarkPerUrl.Select(url => url.LoadTiming).Min());
        public Lazy<TimeSpan> FastestGetHtmlResult => new Lazy<TimeSpan>(() => BenchmarkPerUrl.Select(url => url.GetHtmlResultTiming).Min());
    }

    public class ScrapingBenchmarkResult
    {
        public string Url;
        public TimeSpan GoToUrlTiming;
        public TimeSpan LoadTiming;
        public TimeSpan GetHtmlResultTiming;

        public List<ElementTiming> MetadataExtractionTiming = new();
        public List<ElementTiming> ContentExclusionTiming = new ();
    }
    
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
