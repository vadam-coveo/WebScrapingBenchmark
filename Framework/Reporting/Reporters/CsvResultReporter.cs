using CsvHelper;
using System.Globalization;
using WebScrapingBenchmark.Framework.ScenarioRunner;
using WebScrapingBenchmark.Framework.UrlScrapingResults;

namespace WebScrapingBenchmark.Framework.Reporting.Reporters
{
    internal class CsvResultReporter : IScrapingResultsReporter
    {
        public int Index { get; }
        private IAggregator<ScrapingMetrics> ScrapingMetricsAggregator { get; }

        public CsvResultReporter(IAggregator<ScrapingMetrics> scrapingMetricsAggregator, int index)
        {
            ScrapingMetricsAggregator = scrapingMetricsAggregator;
            Index = index;
        }

        public void ReportResults()
        {
            using (var writer = new StreamWriter("results.csv"))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(ScrapingMetricsAggregator.Items);
                }
            }
        }
    }
}
