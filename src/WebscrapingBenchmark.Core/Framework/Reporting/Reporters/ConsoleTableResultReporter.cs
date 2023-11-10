using System.Data;
using Humanizer;
using WebscrapingBenchmark.Core.Framework.Logging;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Framework.Reporting.Reporters
{
    public class ConsoleTableResultReporter : IScrapingResultsReporter
    {
        private IAggregator<ScrapingMetrics> ScrapingMetricsAggregator { get; }

        public int Index { get; }

        private string ReporterName { get; }

        private Func<ScrapingMetrics, string> Grouping { get; }


        public ConsoleTableResultReporter(int index, string reporterName, Func<ScrapingMetrics, string> grouping, IAggregator<ScrapingMetrics> scrapingMetricsAggregator)
        {
            Index = index;
            ReporterName = reporterName;
            Grouping = grouping;
            ScrapingMetricsAggregator = scrapingMetricsAggregator;
        }

        public void ReportResults()
        {
            ConsoleLogger.Info("\r\r");
            ConsoleLogger.Info("\r\r");
            ConsoleLogger.Warn($"--------------------------------{ReporterName}---------------------------");

            foreach (var scenarioGroup in ScrapingMetricsAggregator.Items.GroupBy(Grouping))
            {
                PrintGroupingHeader(scenarioGroup.Key, scenarioGroup);
                PrintGroupTable(scenarioGroup);
            }
        }

        private void PrintGroupTable(IEnumerable<ScrapingMetrics> scenarioGroup)
        {
            var dt = new DataTable();
            dt.Columns.Add("Web-Scraping Metric");

            var scrapers = ScrapingMetricsAggregator.Items.Select(x => x.ScraperName).Distinct().Count();

            for (var i = 0; i < scrapers; i++)
            {
                dt.Columns.Add($"{i + 1} place".PadLeft(40));
            }


            FillDatarowFromBestToWorstTimings(dt, "GoToUrl", scenarioGroup, result => result.GoToUrlTiming);
            FillDatarowFromBestToWorstTimings(dt, "Load", scenarioGroup, result => result.LoadTiming);
            FillDatarowFromBestToWorstTimings(dt, "Metadata Extraction Time", scenarioGroup, result => result.TotalMetadataExtractionTime.Value);
            FillDatarowFromBestToWorstTimings(dt, "Content Exclusion Time", scenarioGroup, result => result.TotalContentExclusionTime.Value);
            FillDatarowFromBestToWorstTimings(dt, "GetHtmlResult Time", scenarioGroup, result => result.GetHtmlResultTiming);
            FillDatarowFromBestToWorstTimings(dt, "Total Scraping Time", scenarioGroup, result => result.TotalScrapingTime.Value);


            var output = AsciiTableGenerator.CreateAsciiTableFromDataTable(dt);
            Console.Write(output);
        }

        private void PrintGroupingHeader(string headerTitle, IEnumerable<ScrapingMetrics> scenarioGroup)
        {
            var selection = scenarioGroup.Where(result => result.ScraperName == "ActualBaseline");

            // if there's no baselines, that's because we're grouping per scraper, so we should consider the entire group for the headers
            if (!selection.Any())
            {
                selection = scenarioGroup;
            }


            var bytesBefore = selection.Average(x => x.InitialHtmlBytes).Bytes().Humanize();
            var bytesAfter = selection.Average(x => x.FinalHtmlBytes).Bytes().Humanize();

            var reductionRatio = FormatHelper.FormatNumber(selection.Average(x => x.DocumentReductionRatio));
            var exclusionHit = FormatHelper.FormatNumber(selection.Average(x => x.ExclusionsHit), 0);
            var exclusionsFail = FormatHelper.FormatNumber(selection.Average(x => x.ExclusionFail), 0);
            var exclusionHHitRate = FormatHelper.FormatNumber(selection.Average(x => x.ExclusionsHitRate), 2);

            var metadataHit = FormatHelper.FormatNumber(selection.Average(x => x.MetadataHit), 0);
            var metadataFail = FormatHelper.FormatNumber(selection.Average(x => x.MetadataFail), 0);
            var metadataHitRate = FormatHelper.FormatNumber(selection.Average(x => x.MetadataHitRate), 2);

            var metadataBytesExtracted = Math.Round(selection.Average(x => x.MetadataBytesExtractedTotal)).Bytes().Humanize();
            var metadataRatioAccordingToDocumentSize = FormatHelper.FormatNumber(selection.Average(x => x.MetadataRatioAccordingToDocumentSize), 2);

            var nbUrls = selection.Count();

            ConsoleLogger.Info("\r\r");
            ConsoleLogger.Warn($"{headerTitle}{(nbUrls > 1 ? $" ({nbUrls} urls evaluated, numbers are averaged - out)" : "")}");
            ConsoleLogger.Info($"Document size {bytesBefore} initial,  {bytesAfter} after exclusions ({reductionRatio} % reduction)");
            ConsoleLogger.Info($"Exclusions {exclusionHit} matched, {exclusionsFail} failed on average  ({exclusionHHitRate}% success rate) ");
            ConsoleLogger.Info($"Metadata   {metadataHit} matched,  {metadataFail} failed  ({metadataHitRate} % success rate) --> {metadataBytesExtracted}  ({metadataRatioAccordingToDocumentSize} % of initial html bytes)");
            ConsoleLogger.Info("\r");
        }

        private void FillDatarowFromBestToWorstTimings(DataTable dt, string metric, IEnumerable<ScrapingMetrics> dataSet, Func<ScrapingMetrics, TimeSpan> criteria)
        {
            var dr = dt.NewRow();
            var dr2 = dt.NewRow();
            var dr3 = dt.NewRow();
            dr[0] = metric;

            var results = new Dictionary<string, TimeSpan>();

            foreach (var kvp in dataSet.GroupBy(x => x.ScraperName))
            {
                results[kvp.Key] = kvp.Select(criteria).Sum();
            }

            var sorted = results.OrderBy(x => x.Value);

            var worst = sorted.Last().Value;

            var columnIndex = 1;
            foreach (var kvp in sorted)
            {
                dr[columnIndex] = $"{FormatHelper.FormatStrategyName(kvp.Key)}";
                dr2[columnIndex] = $"{FormatHelper.StringifyDifference(kvp.Value, worst)}";
                columnIndex++;
            }

            dt.Rows.Add(dr);
            dt.Rows.Add(dr2);
            dt.Rows.Add(dr3);
        }
    }
}
