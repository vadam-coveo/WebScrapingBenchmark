using WebscrapingBenchmark.Core.Framework.Helpers;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Framework.Reporting.Reporters
{
    public class FilesystemJsonReporter : IScrapingResultsReporter
    {
        public int Index { get; }

        private IAggregator<ScrapingMetrics> ScrapingMetricsAggregator { get; }


        public Func<ScrapingMetrics,bool>? Filter { get; }

        public FilesystemJsonReporter(IAggregator<ScrapingMetrics> scrapingMetricsAggregator, int index)
        {
            ScrapingMetricsAggregator = scrapingMetricsAggregator;
            Index = index;
        }

        public FilesystemJsonReporter(IAggregator<ScrapingMetrics> scrapingMetricsAggregator, Func<ScrapingMetrics, bool> filter, int index)
        {
            ScrapingMetricsAggregator = scrapingMetricsAggregator;
            Filter = filter;
            Index = index;
        }

        public void ReportResults()
        {
            var filtered = Filter == null? ScrapingMetricsAggregator.Items : ScrapingMetricsAggregator.Items.Where(Filter).ToList();

            foreach (var metric  in filtered)
            {
                FilesystemHelper.ToJsonFile(metric, Path.Combine(FilesystemHelper.Solution.MetricsOutputDirectory, GetFilename(metric)));
            }
        }

        public static string GetScraperFilterFileSearchPattern(string scraperName)
        {
            return $"{scraperName}-*.json";
        }

        private static string GetFilename(ScrapingMetrics metric)
        {
            return $"{metric.ScraperName}-{metric.ScenarioName}-{FilesystemHelper.CreateMD5(metric.Url)}.json";
        }
    }
}
