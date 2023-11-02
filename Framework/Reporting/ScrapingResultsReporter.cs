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

                var goToUrlAverage = scenarioGroup.Select(result => result.GoToUrlTiming).Average();
                var goToUrlBest = GetBest(scenarioGroup, result => result.GoToUrlTiming);
                var goToUrlWorst = GetWorst(scenarioGroup, result => result.GoToUrlTiming);
                var loadAverage = scenarioGroup.Select(result => result.LoadTiming).Average();
                var loadBest = GetBest(scenarioGroup, result => result.LoadTiming);
                var loadWorst = GetWorst(scenarioGroup, result => result.LoadTiming);
                var getHtmlResultAverage = scenarioGroup.Select(result => result.GetHtmlResultTiming).Average();
                var getHtmlResultBest = GetBest(scenarioGroup, result => result.GetHtmlResultTiming);
                var getHtmlResultWorst = GetWorst(scenarioGroup, result => result.GetHtmlResultTiming);

                var metadataExtractionAverage = scenarioGroup.Select(result => result.AverageMetadataExtraction.Value).Average();
                var metadataExtractionBest = GetBest(scenarioGroup, result => result.AverageMetadataExtraction.Value);
                var metadataExtractionWorst = GetWorst(scenarioGroup, result => result.AverageMetadataExtraction.Value);
                var contentExclusionAverage = scenarioGroup.Select(result => result.AverageContentExclusion.Value).Average();
                var contentExclusionBest = GetBest(scenarioGroup, result => result.AverageContentExclusion.Value);
                var contentExclusionWorst = GetWorst(scenarioGroup, result => result.AverageContentExclusion.Value);

                var totalScrapingTimeAverage = scenarioGroup.Select(result => result.TotalScrapingTime.Value).Average();
                var totalScrapingTimeBest = GetBest(scenarioGroup, result => result.TotalScrapingTime.Value);
                var totalScrapingTimeWorst = GetWorst(scenarioGroup, result => result.TotalScrapingTime.Value);

                reportingEntries.Add(CreateConsoleReportingEntry("GoToUrl", goToUrlAverage, goToUrlBest, goToUrlWorst, result => result.GoToUrlTiming));
                reportingEntries.Add(CreateConsoleReportingEntry("Load", loadAverage, loadBest, loadWorst, result => result.LoadTiming));
                reportingEntries.Add(CreateConsoleReportingEntry("GetHtmlResult", getHtmlResultAverage, getHtmlResultBest, getHtmlResultWorst, result => result.GetHtmlResultTiming));

                reportingEntries.Add(CreateConsoleReportingEntry("MetadataExtraction", metadataExtractionAverage, metadataExtractionBest, metadataExtractionWorst, result => result.AverageContentExclusion.Value));
                reportingEntries.Add(CreateConsoleReportingEntry("ContentExclusion", contentExclusionAverage, contentExclusionBest, contentExclusionWorst, result => result.AverageContentExclusion.Value));

                reportingEntries.Add(CreateConsoleReportingEntry("TotalScrapingTime", totalScrapingTimeAverage, totalScrapingTimeBest, totalScrapingTimeWorst, result => result.TotalScrapingTime.Value));

                ConsoleLogger.Warn(scenarioGroup.Key);
                ConsoleLogger.WriteLine(Formatter.Format(reportingEntries));
            }
        }

        private static ConsoleReportingEntry CreateConsoleReportingEntry(string metric,
                                                                         TimeSpan average,
                                                                         ScrapingMetrics? fastest,
                                                                         ScrapingMetrics? slowest,
                                                                         Func<ScrapingMetrics, TimeSpan> criteria)
        {
            return new ConsoleReportingEntry(metric,
                                             FormatHelper.StringifyDuration(average),
                                             FormatHelper.StringifyDurationDifference(fastest != null ? criteria(fastest) : TimeSpan.Zero, average),
                                             FormatHelper.FormatStrategyName(fastest?.ScraperName ?? "None"),
                                             FormatHelper.StringifyDurationDifference(slowest != null ? criteria(slowest) : TimeSpan.Zero, average),
                                             FormatHelper.FormatStrategyName(slowest?.ScraperName ?? "None"));
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
