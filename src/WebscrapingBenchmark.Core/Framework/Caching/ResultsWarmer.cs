using WebscrapingBenchmark.Core.Framework.Helpers;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Framework.Caching
{
    public class ResultsWarmer : ICacheWarmer
    {
        private IAggregator<ScrapingMetrics> ScrapingMetricsAggregator { get; }

        public string Filter { get; }
        public ResultsWarmer(IAggregator<ScrapingMetrics> scrapingMetricsAggregator, string filter)
        {
            ScrapingMetricsAggregator = scrapingMetricsAggregator;
            Filter = filter;
        }

        public void WarmCache()
        {
            var files = Directory.GetFiles(FilesystemHelper.Solution.MetricsOutputDirectory, Filter);

            foreach (var filepath in files)
            {
                var metric = FilesystemHelper.FromJsonFile<ScrapingMetrics>(filepath);
                ScrapingMetricsAggregator.Aggregate(metric);
            }
        }
    }
}
