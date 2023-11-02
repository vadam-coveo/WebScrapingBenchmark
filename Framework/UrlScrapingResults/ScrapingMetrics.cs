using CsvHelper.Configuration.Attributes;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults
{
    public class ScrapingMetrics : ScrapingOutput
    {
        [Ignore]
        public List<ContentExclusionKpi> ExclusionKpi => _exclusionKpi ??= GetExclusionsKpi();
        private List<ContentExclusionKpi>? _exclusionKpi;

        public long ExcludedBytesPerTick => ExcludedBytes / TotalContentExclusionTime.Value.Ticks;

        public ScrapingMetrics(string url, string scenarioName, string scraperName) : base(url, scenarioName, scraperName)
        {
        }

        private List<ContentExclusionKpi> GetExclusionsKpi()
        {
            var output = new List<ContentExclusionKpi>();

            foreach (var exclusion in ContentExclusions)
            {
                var matchingTiming = ContentExclusionTiming.FirstOrDefault(x => x.SelectorName == exclusion.Key);
                if (matchingTiming == null) continue; // should not happen

                output.Add(new ContentExclusionKpi(Url, 
                    ScenarioName, 
                    ScraperName, 
                    exclusion.Key,
                    matchingTiming.Duration,
                    ExcludedBytes,
                    InitialHtmlBytes
                    ));
            }
            return new List<ContentExclusionKpi>();
        }

        public class ContentExclusionKpi : BaseUrlScrapingResult
        {
            public string SelectorPath { get; }
            public long ExcludedBytes { get; set; }
            public TimeSpan Duration { get; set; }
            public decimal ExcludedBytesPerTick { get; set; }
            public decimal PercentageExcludedFromInitial { get; set; }

            public ContentExclusionKpi(string url, string scenarioName, string scraperName, string selectorPath, TimeSpan duration, long excludedBytes, long initalPageBytes) : base(url, scenarioName, scraperName)
            {
                SelectorPath = selectorPath;
                Duration = duration;
                ExcludedBytes = excludedBytes;

                ExcludedBytesPerTick = Convert.ToDecimal(ExcludedBytes) / Convert.ToDecimal(Duration.Ticks);
                PercentageExcludedFromInitial = (Convert.ToDecimal(ExcludedBytes) / Convert.ToDecimal(initalPageBytes)) * 100;
            }
        }
    }
}
