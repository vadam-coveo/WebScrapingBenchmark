﻿
namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class Benchmark
    {
        public string ScenarioName;
        public string ScraperName;

        public List<ScrapingBenchmarkResult> BenchmarkPerUrl = new();
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
}
