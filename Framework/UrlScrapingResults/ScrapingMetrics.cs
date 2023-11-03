using CsvHelper.Configuration.Attributes;
using System.ComponentModel;
using WebScrapingBenchmark.Framework.Logging;
using WebScrapingBenchmark.Framework.Reporting;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults
{
    public class ScrapingMetrics : ScrapingOutput
    {
        [Ignore]
        public List<ContentExclusionKpi> ExclusionKpis => _exclusionKpi ?? GetExclusionsKpi();
        private List<ContentExclusionKpi>? _exclusionKpi;

        public List<MetadataKpi> MetadataKpis => _metadataKpi ??= GetMetadataKpis();
        private List<MetadataKpi>? _metadataKpi;

        public long ExcludedBytes => ExclusionKpis.Sum(x => x.ExcludedBytes);

        public decimal ExcludedBytesPerTick => GetRatio(ExcludedBytes, TotalContentExclusionTime.Value.Ticks);

        public decimal ExclusionsHitRate => GetRatio(ExclusionsHit, ExclusionKpis.Count);
        public int ExclusionsHit => ExclusionKpis.Count(x => x.ExcludedBytes > 0);
        public int ExclusionFail => ExclusionKpis.Count - ExclusionsHit;
        public long ExclusionsBytesRemovedTotal => ExclusionKpis.Sum(x => x.ExcludedBytes);
        public decimal DocumentReductionRatio => GetRatio(InitialHtmlBytes- FinalHtmlBytes, InitialHtmlBytes);


        public int MetadataHit => MetadataKpis.Count(x => x.NbMatched > 0);
        public int MetadataFail => MetadataKpis.Count - MetadataHit;
        public decimal MetadataHitRate => GetRatio(MetadataHit, MetadataKpis.Count);
        public long MetadataBytesExtractedTotal => MetadataKpis.Sum(x => x.TotalBytesMatched);
        public decimal MetadataRatioAccordingToDocumentSize => GetRatio(MetadataBytesExtractedTotal, InitialHtmlBytes);

        [CsvHelper.Configuration.Attributes.TypeConverter(typeof(TimespanConverter))]
        public TimeSpan TotalTimeWastedOnUnmatchedExclusions =>
            TimeSpan.FromTicks((ExclusionKpis.Where(x => x.ExcludedBytes == 0).Sum(x => x.Duration.Ticks)));

        public ScrapingMetrics(string url, string scenarioName, string scraperName) : base(url, scenarioName, scraperName)
        {
        }

        private static decimal GetRatio(long nominator, long denominator, int precision = 2)
        {
            if (denominator == 0) denominator = 1;

            return Math.Round(Convert.ToDecimal(nominator * 100) / Convert.ToDecimal(denominator), precision);
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
                    exclusion.Value,
                    InitialHtmlBytes
                    ));
            }

            return output;
        }

        private List<MetadataKpi> GetMetadataKpis()
        {
            var output = new List<MetadataKpi>();

            foreach (var meta in Metadata)
            {
                var matchingTiming = MetadataExtractionTiming.FirstOrDefault(x => x.SelectorName == meta.Key);
                if (matchingTiming == null) continue; // should not happen

                output.Add(new MetadataKpi(Url,
                    ScenarioName,
                    ScraperName,
                    meta.Key,
                    matchingTiming.Duration,
                    meta.Value));
            }

            return output;
        }

        public class MetadataKpi : BaseUrlScrapingResult
        {
            public string SelectorPath { get; }
            public int NbMatched { get; }
            public long TotalBytesMatched { get; }
            public TimeSpan Duration { get; }
            public IEnumerable<string> Content { get; }

            public MetadataKpi(string url, string scenarioName, string scraperName, string selectorPath, TimeSpan duration, IEnumerable<string> content) : base(url, scenarioName, scraperName)
            {
                SelectorPath = selectorPath;
                Duration = duration;
                Content = content;
                NbMatched = content.Count();
                TotalBytesMatched = content.Sum(FormatHelper.GetBytes);
            }
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
