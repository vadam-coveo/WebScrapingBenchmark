using System.Globalization;
using CsvHelper;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Framework.Reporting.Reporters
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
