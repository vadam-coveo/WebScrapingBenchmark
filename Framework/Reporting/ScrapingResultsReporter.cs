using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AsciiTableFormatter;
using CsvHelper;
using Humanizer;
using WebScrapingBenchmark.Framework.Logging;
using WebScrapingBenchmark.Framework.ScenarioRunner;
using WebScrapingBenchmark.Framework.UrlScrapingResults;
using WebScrapingBenchmark.WebScrapingStrategies;

namespace WebScrapingBenchmark.Framework.Reporting
{
    public class ScrapingResultsReporter : IScrapingResultsReporter
    {
        private IAggregator<ScrapingMetrics> ScrapingMetricsAggregator { get; }

        public ScrapingResultsReporter(IAggregator<ScrapingMetrics> scrapingMetricsAggregator)
        {
            ScrapingMetricsAggregator = scrapingMetricsAggregator;
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

            ConsoleLogger.WriteLine("Results:");

            var resultsGroupByScenarioIdentifier =
                ScrapingMetricsAggregator.Items.GroupBy(item => item.ScenarioIdentifier);

            foreach (var scenarioGroup in resultsGroupByScenarioIdentifier)
            {
                var reportingEntries = new List<ConsoleReportingEntry>();

                reportingEntries.Add(CreateConsoleReportingEntry("GoToUrl", scenarioGroup, result => result.GoToUrlTiming));
                reportingEntries.Add(CreateConsoleReportingEntry("Load", scenarioGroup, result => result.LoadTiming));
                reportingEntries.Add(CreateConsoleReportingEntry("GetHtmlResult", scenarioGroup, result => result.GetHtmlResultTiming));

                reportingEntries.Add(CreateConsoleReportingEntry("MetadataExtraction", scenarioGroup, result => result.AverageMetadataExtraction.Value));
                reportingEntries.Add(CreateConsoleReportingEntry("ContentExclusion", scenarioGroup, result => result.AverageContentExclusion.Value));

                reportingEntries.Add(CreateConsoleReportingEntry("TotalScrapingTime", scenarioGroup, result => result.TotalScrapingTime.Value));

                ConsoleLogger.Warn(scenarioGroup.Key);
                ConsoleLogger.Info($"Initial HTML in bytes: {scenarioGroup.ElementAt(0).InitialHtmlBytes.Bytes().Humanize()}");
                ConsoleLogger.Info($"Final HTML in bytes:   {scenarioGroup.First(result => result.ScraperName == nameof(BaselineStrategy)).FinalHtmlBytes.Bytes().Humanize()}");
                ConsoleLogger.WriteLine(Formatter.Format(reportingEntries));
            }
        }

        private static ConsoleReportingEntry CreateConsoleReportingEntry(string metric,
                                                                         IEnumerable<ScrapingMetrics> group,
                                                                         Func<ScrapingMetrics, TimeSpan> criteria)
        {
            var average = group.Select(criteria).Average();
            var best = GetBest(group, criteria);
            var worst = GetWorst(group, criteria);

            return new ConsoleReportingEntry(metric,
                                             FormatHelper.StringifyDuration(average),
                                             FormatHelper.StringifyDurationDifference(best != null ? criteria(best) : TimeSpan.Zero, average),
                                             FormatHelper.FormatStrategyName(best?.ScraperName ?? "None"),
                                             FormatHelper.StringifyDurationDifference(worst != null ? criteria(worst) : TimeSpan.Zero, average),
                                             FormatHelper.FormatStrategyName(worst?.ScraperName ?? "None"));
        }

        private static ScrapingMetrics? GetBest(IEnumerable<ScrapingMetrics> group, Func<ScrapingMetrics, TimeSpan> criteria)
        {
            return group.OrderBy(criteria).FirstOrDefault(result => criteria.Invoke(result) != TimeSpan.Zero);
        }

        private static ScrapingMetrics? GetWorst(IEnumerable<ScrapingMetrics> group, Func<ScrapingMetrics, TimeSpan> criteria)
        {
            return group.OrderByDescending(criteria).FirstOrDefault(result => criteria.Invoke(result) != TimeSpan.Zero);
        }
    }
}
