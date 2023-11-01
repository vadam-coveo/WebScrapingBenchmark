
namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class Benchmark
    {
        public string ScenarioName;
        public string ScraperName;

        public List<ScrapingBenchmarkResult> BenchmarkPerUrl = new List<ScrapingBenchmarkResult>();
    }

    public class ScrapingBenchmarkResult
    {
        public string Url;
        public TimeSpan GoToUrlTiming;
        public TimeSpan LoadTiming;
        public TimeSpan GetHtmlResultTiming;

        public List<ElementTiming> MetadataExtractionTiming = new List<ElementTiming>();
        public List<ElementTiming> ContentExclusionTiming = new List<ElementTiming>();
    }
    
    public class ElementTiming
    {
        public string SelectorName;
        public TimeSpan Duration;
    }
}
