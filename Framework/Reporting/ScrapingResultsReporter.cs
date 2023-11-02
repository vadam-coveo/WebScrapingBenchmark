using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using CsvHelper;
using Humanizer;
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
                builder.AppendLine($"Scenario Identifier: {scenarioGroup.Key}");

                builder.AppendLine($"Average GoToUrl: {scenarioGroup.Select(result => result.GoToUrlTiming).Average().Humanize(3)}");
                builder.AppendLine($"Average Load: {scenarioGroup.Select(result => result.LoadTiming).Average().Humanize(3)}");
                builder.AppendLine($"Average GetHtmlResult: {scenarioGroup.Select(result => result.GetHtmlResultTiming).Average().Humanize(3)}");

                builder.AppendLine($"Average MetadataExtraction: {scenarioGroup.Select(result => result.AverageMetadataExtraction.Value).Average().Humanize(3)}");
                builder.AppendLine($"Average ContentExclusion: {scenarioGroup.Select(result => result.AverageContentExclusion.Value).Average().Humanize(3)}");

                //builder.AppendLine($"Average TotalScrapingTime: {scenarioGroup.Select(result => result.TotalScrapingTime.Value).Average().Humanize(3)}");
                
                builder.AppendLine($"Fastest GoToUrl: {OutputBest(scenarioGroup, result => result.GoToUrlTiming)}");
                builder.AppendLine($"Fastest Load: {OutputBest(scenarioGroup, result => result.LoadTiming)}");
                builder.AppendLine($"Fastest GetHtmlResult: {OutputBest(scenarioGroup, result => result.GetHtmlResultTiming)}");

                Console.WriteLine(builder);
            }
        }

        private static string OutputBest(IEnumerable<ScrapingTimingResults> group, Func<ScrapingTimingResults, TimeSpan> criteria)
        {
            var fastest = group.OrderBy(criteria).First();
            return $"{fastest.ScraperName} => {fastest.GoToUrlTiming.Humanize(3)}";
        }
    }
}
