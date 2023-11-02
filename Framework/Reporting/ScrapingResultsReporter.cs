using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AsciiTableFormatter;
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
                var reportingEntries = new List<ConsoleReportingEntry>();

                var goToUrlAverage = scenarioGroup.Select(result => result.GoToUrlTiming).Average();
                var goToUrlBest = GetBest(scenarioGroup, result => result.GoToUrlTiming);
                var goToUrlWorst = GetWorst(scenarioGroup, result => result.GoToUrlTiming);
                var loadAverage = scenarioGroup.Select(result => result.LoadTiming).Average();
                var loadBest = GetBest(scenarioGroup, result => result.LoadTiming);
                var loadWorst = GetWorst(scenarioGroup, result => result.LoadTiming);
                var getHtmlResultAverage = scenarioGroup.Select(result => result.GetHtmlResultTiming).Average();
                var getHtmlResultBest = GetBest(scenarioGroup, result => result.GetHtmlResultTiming);
                var getHtmlResultWorst = GetWorst(scenarioGroup, result => result.GetHtmlResultTiming);

                reportingEntries.Add(new ConsoleReportingEntry("GoToUrl",
                                                               FormatHelper.StringifyDuration(goToUrlAverage),
                                                               FormatHelper.StringifyDuration(goToUrlBest?.GoToUrlTiming ?? TimeSpan.Zero - goToUrlAverage),
                                                               goToUrlBest?.ScraperName ?? "None",
                                                               FormatHelper.StringifyDuration(goToUrlWorst?.GoToUrlTiming ?? TimeSpan.Zero - goToUrlAverage),
                                                               goToUrlWorst?.ScraperName ?? "None"));

                reportingEntries.Add(new ConsoleReportingEntry("Load",
                                                               FormatHelper.StringifyDuration(loadAverage),
                                                               FormatHelper.StringifyDuration(loadBest?.GoToUrlTiming ?? TimeSpan.Zero - loadAverage),
                                                               loadBest?.ScraperName ?? "None",
                                                               FormatHelper.StringifyDuration(loadWorst?.GoToUrlTiming ?? TimeSpan.Zero - loadAverage),
                                                               loadWorst?.ScraperName ?? "None"));

                reportingEntries.Add(new ConsoleReportingEntry("GetHtmlResult",
                                                               FormatHelper.StringifyDuration(getHtmlResultAverage),
                                                               FormatHelper.StringifyDuration(getHtmlResultBest?.GoToUrlTiming ?? TimeSpan.Zero - getHtmlResultAverage),
                                                               getHtmlResultBest?.ScraperName ?? "None",
                                                               FormatHelper.StringifyDuration(getHtmlResultWorst?.GoToUrlTiming ?? TimeSpan.Zero - getHtmlResultAverage),
                                                               getHtmlResultWorst?.ScraperName ?? "None"));

                Console.WriteLine(scenarioGroup.Key);
                Console.WriteLine(Formatter.Format(reportingEntries));

                /*var builder = new StringBuilder();
                builder.AppendLine($"{scenarioGroup.Key}");

                builder.AppendLine($"\t{FormatHelper.StringifyDuration(scenarioGroup.Select(result => result.AverageMetadataExtraction.Value).Average())}\tAverage MetadataExtraction");
                builder.AppendLine($"\t{FormatHelper.StringifyDuration(scenarioGroup.Select(result => result.AverageContentExclusion.Value).Average())}\tAverage ContentExclusion");

                builder.AppendLine($"\t{FormatHelper.StringifyDuration(scenarioGroup.Select(result => result.TotalScrapingTime.Value).Average())}\tAverage TotalScrapingTime");

                builder.AppendLine($"\t{OutputBest(scenarioGroup, result => result.GoToUrlTiming)}\t\tFastest GoToUrl");
                builder.AppendLine($"\t{OutputBest(scenarioGroup, result => result.LoadTiming)}\t\tFastest Load");
                builder.AppendLine($"\t{OutputBest(scenarioGroup, result => result.GetHtmlResultTiming)}\t\tFastest GetHtmlResult");

                builder.AppendLine($"\t{OutputBest(scenarioGroup, result => result.TotalMetadataExtractionTime.Value)}\tFastest MetadataExtraction");
                builder.AppendLine($"\t{OutputBest(scenarioGroup, result => result.TotalContentExclusionTime.Value)}\tFastest ContentExclusion");

                builder.AppendLine($"\t{OutputBest(scenarioGroup, result => result.TotalScrapingTime.Value)}\tFastest TotalScrapingTime");

                Console.WriteLine(builder);*/
            }
        }

        private static string OutputBest(IEnumerable<ScrapingTimingResults> group, Func<ScrapingTimingResults, TimeSpan> criteria)
        {
            var best = GetBest(group, criteria);
            return best != null ? $"{FormatHelper.StringifyDuration(criteria.Invoke(best))} <= {best.ScraperName}" : "None";
        }

        private static ScrapingTimingResults? GetBest(IEnumerable<ScrapingTimingResults> group, Func<ScrapingTimingResults, TimeSpan> criteria)
        {
            return group.OrderBy(criteria).FirstOrDefault(result => criteria.Invoke(result) != TimeSpan.Zero);
        }

        private static ScrapingTimingResults? GetWorst(IEnumerable<ScrapingTimingResults> group, Func<ScrapingTimingResults, TimeSpan> criteria)
        {
            return group.OrderByDescending(criteria).FirstOrDefault(result => criteria.Invoke(result) != TimeSpan.Zero);
        }
    }
}
