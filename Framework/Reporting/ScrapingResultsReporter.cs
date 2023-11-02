using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using CsvHelper;
using Humanizer;
using WebScrapingBenchmark.Framework.Logging;
using WebScrapingBenchmark.Framework.ScenarioRunner;
using WebScrapingBenchmark.Framework.ScrapingResultComparing;
using WebScrapingBenchmark.Framework.UrlScrapingResults;

namespace WebScrapingBenchmark.Framework.Reporting
{
    public class ScrapingResultsReporter : IScrapingResultsReporter
    {
        private IAggregator<ScrapingOutput> ScrapingOutputAggregator { get; }
        private IAggregator<ScrapingTimingResults> ScrapingTimingAggregator { get; }

        public ScrapingResultsReporter(IAggregator<ScrapingOutput> scrapingOutputAggregator, IAggregator<ScrapingTimingResults> scrapingTimingAggregator)
        {
            ScrapingOutputAggregator = scrapingOutputAggregator;
            ScrapingTimingAggregator = scrapingTimingAggregator;
        }

        public void ReportResults()
        {
            using (var writer = new StreamWriter("results.csv"))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(ScrapingTimingAggregator.Items);
                }
            }

            Console.WriteLine("Results:");

            var resultsGroupByScenarioIdentifier =
                ScrapingTimingAggregator.Items.GroupBy(item => item.ScenarioIdentifier);

            foreach (var scenarioGroup in resultsGroupByScenarioIdentifier)
            {
                var builder = new StringBuilder();
                builder.AppendLine($"{scenarioGroup.Key}");

                builder.AppendLine($"\t{FormatHelper.StringifyDuration(scenarioGroup.Select(result => result.GoToUrlTiming).Average())}\tAverage GoToUrl");
                builder.AppendLine($"\t{FormatHelper.StringifyDuration(scenarioGroup.Select(result => result.LoadTiming).Average())}\tAverage Load");
                builder.AppendLine($"\t{FormatHelper.StringifyDuration(scenarioGroup.Select(result => result.GetHtmlResultTiming).Average())}\tAverage GetHtmlResult");

                builder.AppendLine($"\t{FormatHelper.StringifyDuration(scenarioGroup.Select(result => result.AverageMetadataExtraction.Value).Average())}\tAverage MetadataExtraction");
                builder.AppendLine($"\t{FormatHelper.StringifyDuration(scenarioGroup.Select(result => result.AverageContentExclusion.Value).Average())}\tAverage ContentExclusion");

                //builder.AppendLine($"\t{FormatHelper.StringifyDuration(scenarioGroup.Select(result => result.TotalScrapingTime.Value).Average())}\tAverage TotalScrapingTime");

                builder.AppendLine($"\t{OutputBest(scenarioGroup, result => result.GoToUrlTiming)}\t\tFastest GoToUrl");
                builder.AppendLine($"\t{OutputBest(scenarioGroup, result => result.LoadTiming)}\t\tFastest Load");
                builder.AppendLine($"\t{OutputBest(scenarioGroup, result => result.GetHtmlResultTiming)}\t\tFastest GetHtmlResult");

                Console.WriteLine(builder);
            }
        }

        private static string OutputBest(IEnumerable<ScrapingTimingResults> group, Func<ScrapingTimingResults, TimeSpan> criteria)
        {
            var fastest = group.OrderBy(criteria).First(result => criteria.Invoke(result) != TimeSpan.Zero);
            return $"{FormatHelper.StringifyDuration(criteria.Invoke(fastest))} <= {fastest.ScraperName}";
        }
    }
}
