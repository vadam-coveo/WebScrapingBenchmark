using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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


            ReportResultsV2();
            return;
            ConsoleLogger.WriteLine("Results:");

            var resultsGroupByScenarioIdentifier =
                ScrapingMetricsAggregator.Items.GroupBy(item => item.ScenarioIdentifier);

            foreach (var scenarioGroup in resultsGroupByScenarioIdentifier)
            {
                var reportingEntries = new List<ConsoleReportingEntry>();

                reportingEntries.Add(CreateConsoleReportingEntry("GoToUrl", scenarioGroup, result => result.GoToUrlTiming));
                reportingEntries.Add(CreateConsoleReportingEntry("Load", scenarioGroup, result => result.LoadTiming));
                reportingEntries.Add(CreateConsoleReportingEntry("GetHtmlResult", scenarioGroup, result => result.GetHtmlResultTiming));

                reportingEntries.Add(CreateConsoleReportingEntry("MetadataExtraction", scenarioGroup, result => result.TotalMetadataExtractionTime.Value));
                reportingEntries.Add(CreateConsoleReportingEntry("ContentExclusion", scenarioGroup, result => result.TotalContentExclusionTime.Value));

                reportingEntries.Add(CreateConsoleReportingEntry("TotalScrapingTime", scenarioGroup, result => result.TotalScrapingTime.Value));

                ConsoleLogger.Warn(scenarioGroup.Key);
                ConsoleLogger.Info($"Initial HTML in bytes: {scenarioGroup.ElementAt(0).InitialHtmlBytes.Bytes().Humanize()}");
                ConsoleLogger.Info($"Final HTML in bytes:   {scenarioGroup.First(result => result.ScraperName == nameof(Baseline)).FinalHtmlBytes.Bytes().Humanize()}");
                ConsoleLogger.WriteLine(Formatter.Format(reportingEntries));
            }
        }

        public void ReportResultsV2()
        {
            foreach (var scenarioGroup in ScrapingMetricsAggregator.Items.GroupBy(item => item.ScenarioIdentifier))
            {
                var dt = new DataTable();
                dt.Columns.Add("Web-Scraping Metric");

                var scrapers = ScrapingMetricsAggregator.Items.Select(x => x.ScraperName).Distinct().Count();

                for (var i = 0; i < scrapers; i++)
                {
                    dt.Columns.Add($"{i + 1} place".PadLeft(40));
                }


                FillDatarowFromBestToWorst(dt, "GoToUrl", scenarioGroup, result => result.GoToUrlTiming);
                FillDatarowFromBestToWorst(dt, "Load", scenarioGroup, result => result.LoadTiming);
                FillDatarowFromBestToWorst(dt, "Metadata Extraction Time", scenarioGroup, result => result.TotalMetadataExtractionTime.Value);
                FillDatarowFromBestToWorst(dt, "Content Exclusion Time", scenarioGroup, result => result.TotalContentExclusionTime.Value);
                FillDatarowFromBestToWorst(dt, "GetHtmlResult Time", scenarioGroup, result => result.GetHtmlResultTiming);
                FillDatarowFromBestToWorst(dt, "Total Scraping Time", scenarioGroup, result => result.TotalScrapingTime.Value);


                var output = AsciiTableGenerator.CreateAsciiTableFromDataTable(dt);

                var baseline = scenarioGroup.First(result => result.ScraperName == nameof(Baseline));
                var bytesBefore = baseline.InitialHtmlBytes.Bytes().Humanize();
                var bytesAfter = baseline.FinalHtmlBytes.Bytes().Humanize();

                ConsoleLogger.Info("\r\r");
                ConsoleLogger.Warn(scenarioGroup.Key);
                ConsoleLogger.Info($"Document size {bytesBefore} initial,  {bytesAfter} after exclusions ({baseline.DocumentReductionRatio}% reduction)");
                ConsoleLogger.Info($"Exclusions {baseline.ExclusionsHit} matched, {baseline.ExclusionFail} failed  ({baseline.ExclusionsHitRate}% sucess rate) ");
                ConsoleLogger.Info($"Metadata   {baseline.MetadataHit} matched,  {baseline.MetadataFail} failed  ({baseline.MetadataHitRate} % sucess rate) --> {baseline.MetadataBytesExtractedTotal.Bytes().Humanize()}  ({baseline.MetadataRatioAccordingToDocumentSize} % of initial html bytes)");
                ConsoleLogger.Info("\r");
                Console.Write(output);
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


        private void FillDatarowFromBestToWorst(DataTable dt, string metric, IEnumerable<ScrapingMetrics> group, Func<ScrapingMetrics, TimeSpan> criteria)
        {
            var dr = dt.NewRow();
            var dr2 = dt.NewRow();
            var dr3 = dt.NewRow();
            dr[0] = metric;

            var ordered = group.OrderBy(criteria).ToList();

            var worst = criteria.Invoke(ordered.Last());

            var columnIndex = 1;
            foreach (var soretedMetric in ordered)
            {
                dr[columnIndex] = $"{FormatHelper.FormatStrategyName(soretedMetric?.ScraperName ?? "None")}";
                dr2[columnIndex] = $"{FormatHelper.StringifyDifference(criteria.Invoke(soretedMetric), worst)}";
                columnIndex++;
            }

            dt.Rows.Add(dr);
            dt.Rows.Add(dr2);
            dt.Rows.Add(dr3);
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
