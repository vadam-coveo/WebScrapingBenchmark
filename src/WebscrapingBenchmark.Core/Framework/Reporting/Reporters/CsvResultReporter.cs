using System.Globalization;
using CsvHelper;
using WebscrapingBenchmark.Core.Framework.Helpers;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Framework.Reporting.Reporters
{
    public class CsvResultReporter : IScrapingResultsReporter
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
            var filename = FilesystemHelper.ReplaceInvalidChars($"{DateTime.Now:s} - results.csv");

            var path = Path.Combine(FilesystemHelper.Solution.CsvOutputDirectory, filename);

            using (var writer = new StreamWriter(path))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(ScrapingMetricsAggregator.Items);
                }
            }
        }
    }
}
