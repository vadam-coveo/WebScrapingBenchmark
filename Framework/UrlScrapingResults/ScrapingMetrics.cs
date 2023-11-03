using CsvHelper.Configuration.Attributes;
using System.ComponentModel;
using WebScrapingBenchmark.Framework.Reporting;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults
{
    public class ScrapingMetrics : ScrapingOutput
    {
        [Ignore]
        public List<ContentExclusionKpi> ExclusionKpi => _exclusionKpi ??= GetExclusionsKpi();
        private List<ContentExclusionKpi>? _exclusionKpi;

        public long ExcludedBytesPerTick => ExcludedBytes / TotalContentExclusionTime.Value.Ticks;
        public decimal ExcludedRatio => (Convert.ToDecimal(ExcludedBytes)/ Convert.ToDecimal(InitialHtmlBytes)) * 100;
        public int NbSuccessfulExclusions => ContentExclusions.Count(x => x.Value > 0);
        public int NbUnmatchedExclusions => ContentExclusions.Count - NbSuccessfulExclusions;

        [CsvHelper.Configuration.Attributes.TypeConverter(typeof(TimespanConverter))]
        public TimeSpan TotalTimeWastedOnUnmatchedExclusions =>
            TimeSpan.FromTicks((ExclusionKpi.Where(x => x.ExcludedBytes == 0).Sum(x => x.Duration.Ticks)));

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
