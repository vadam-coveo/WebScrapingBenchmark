using System.Data;
using Humanizer;
using WebscrapingBenchmark.Core.Framework.Helpers;
using WebscrapingBenchmark.Core.Framework.Logging;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Framework.Reporting.Reporters
{
    public class ConsoleTableResultReporter : IScrapingResultsReporter
    {
        protected IAggregator<ScrapingMetrics> ScrapingMetricsAggregator { get; }

        public int Index { get; }

        protected string ReporterName { get; }

        protected Func<ScrapingMetrics, string> Grouping { get; }


        public ConsoleTableResultReporter(int index, string reporterName, Func<ScrapingMetrics, string> grouping, IAggregator<ScrapingMetrics> scrapingMetricsAggregator)
        {
            Index = index;
            ReporterName = reporterName;
            Grouping = grouping;
            ScrapingMetricsAggregator = scrapingMetricsAggregator;
        }

        public virtual void ReportResults()
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

        protected string[]? OrderedScrapers;

        protected void BuildOrderedHeaders(IEnumerable<ScrapingMetrics> scenarios)
        {
            if (OrderedScrapers == null)
                OrderedScrapers = ScrapingMetricsAggregator.Items.Select(x => x.ScraperName).Distinct().OrderBy(x => x)
                    .ToArray();
        }

        protected DataTable InitDataTable(IEnumerable<ScrapingMetrics> scenarioGroup, string firstColumnName)
        {
            var dt = new DataTable();
            dt.Columns.Add(firstColumnName);

            BuildOrderedHeaders(scenarioGroup);

            for (var i = 0; i < OrderedScrapers.Length; i++)
            {
                dt.Columns.Add($"{OrderedScrapers[i]}");
            }

            return dt;
        }

        private void PrintGroupTable(IEnumerable<ScrapingMetrics> scenarioGroup)
        {
            var dt = InitDataTable(scenarioGroup, "Web-Scraping Metric");


            FillDatarowForTimingMetric(dt, "GoToUrl", scenarioGroup, result => result.GoToUrlTiming);
            FillDatarowForTimingMetric(dt, "Load", scenarioGroup, result => result.LoadTiming);
            FillDatarowForTimingMetric(dt, "Metadata Extraction Time", scenarioGroup, result => result.TotalMetadataExtractionTime.Value);
            FillDatarowForTimingMetric(dt, "Content Exclusion Time", scenarioGroup, result => result.TotalContentExclusionTime.Value);
            FillDatarowForTimingMetric(dt, "GetHtmlResult Time", scenarioGroup, result => result.GetHtmlResultTiming);

            AddBlankRow(dt);
            FillBytesDiffDataRow(dt, "Exclusions diff", scenarioGroup, x => x.FinalHtmlBytes, x => x.ExclusionsHit);
            FillBytesDiffDataRow(dt, "Metadata diff", scenarioGroup, x => x.MetadataBytesExtractedTotal, x => x.Metadata.Sum(m=> m.Value.Count()));

            AddBlankRow(dt);
            FillDatarowForTimingMetric(dt, "Total Scraping Time", scenarioGroup, result => result.TotalScrapingTime.Value);

            var output = AsciiTableGenerator.CreateAsciiTableFromDataTable(dt);
            Console.Write(output);
        }

        private void AddBlankRow(DataTable dt)
        {
            var row = dt.NewRow();
            dt.Rows.Add(row);
        }

        private void PrintGroupingHeader(string headerTitle, IEnumerable<ScrapingMetrics> scenarioGroup)
        {
            var selection = scenarioGroup.Where(IsBaseline);

            // if there's no baselines, that's because we're grouping per scraper, so we should consider the entire group for the headers
            if (!selection.Any())
            {
                selection = scenarioGroup;
            }

            var bytesBefore = selection.Average(x => x.InitialHtmlBytes).Bytes().Humanize();
            var bytesAfter = selection.Average(x => x.FinalHtmlBytes).Bytes().Humanize();

            var reductionRatio = FormatHelper.FormatNumber(selection.Average(x => x.DocumentReductionRatio),2);
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

        protected void FillDatarowForTimingMetric(DataTable dt, string metric, IEnumerable<ScrapingMetrics> dataSet, Func<ScrapingMetrics, TimeSpan> selector)
        {
            var dr = dt.NewRow();
            dr[0] = metric;

            var results = new Dictionary<string, TimeSpan>();

            foreach (var kvp in dataSet.GroupBy(x => x.ScraperName))
            {
                results[kvp.Key] = kvp.Select(selector).Sum();
            }

            var baseline = results[FilesystemHelper.BaselineExecutorStrategyName];

            var columnIndex = 1;
            foreach (var scraper in OrderedScrapers!)
            {
                dr[columnIndex] = $"{FormatHelper.StringifyDifference(results[scraper], baseline)}";
                columnIndex++;
            }

            dt.Rows.Add(dr);
        }

        protected void FillBytesDiffDataRow(DataTable dt, string metric, IEnumerable<ScrapingMetrics> dataSet, Func<ScrapingMetrics, long> bytesSelector, Func<ScrapingMetrics, int> hitSelector)
        {
            var baselineBytes = dataSet.Where(IsBaseline).Sum(bytesSelector);
            var baselineHits = dataSet.Where(IsBaseline).Sum(hitSelector);

            var dr = dt.NewRow();
            dr[0] = metric;

            var columnIndex = 1;

            foreach (var scraper in OrderedScrapers!)
            {
                var bytes = dataSet.Where(x => x.ScraperName == scraper).Sum(bytesSelector);
                var hits = dataSet.Where(x => x.ScraperName == scraper).Sum(hitSelector);

                dr[columnIndex] = FormatHelper.FormatTwoColumns(FormatHitCount(hits, baselineHits), FormatBytesDiff(bytes - baselineBytes));

                columnIndex++;
            }


            dt.Rows.Add(dr);
        }

        protected string FormatBytesDiff(long bytesDiffWithBaseline)
        {
            return bytesDiffWithBaseline switch
            {
                > 0 => " + " + bytesDiffWithBaseline.Bytes().Humanize(),
                < 0 => " - " + Math.Abs(bytesDiffWithBaseline).Bytes().Humanize(),
                _ => ""
            };
        }

        protected string FormatHitCount(int calculated, int baseline)
        {
            var diff = calculated - baseline;

            return diff switch
            {
                > 0 => $" + {diff} hits",
                < 0 => $" - {diff} hits",
                _ => $"{calculated} hits"
            };
        }


        protected bool IsBaseline(ScrapingMetrics metric)
        {
            return metric.ScraperName == FilesystemHelper.BaselineExecutorStrategyName;
        }
    }
}
