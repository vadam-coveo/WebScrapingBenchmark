using WebscrapingBenchmark.Core.Framework.Logging;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Framework.Reporting.Reporters
{
    public class SingleCriteriaConsoleTableReporter : ConsoleTableResultReporter
    {
        private Func<ScrapingMetrics, TimeSpan> _criteria;

        public SingleCriteriaConsoleTableReporter(int index, string reporterName, Func<ScrapingMetrics, string> grouping, Func<ScrapingMetrics, TimeSpan> criteria, IAggregator<ScrapingMetrics> scrapingMetricsAggregator) 
            : base(index, reporterName, grouping, scrapingMetricsAggregator)
        {
            _criteria = criteria;
        }

        public override void ReportResults()
        {
            ConsoleLogger.Info("\r\r");
            ConsoleLogger.Info("\r\r");
            ConsoleLogger.Warn($"--------------------------------{ReporterName}---------------------------");

            
            var dt = InitDataTable(ScrapingMetricsAggregator.Items, "Group");
            
            foreach (var scenarioGroup in ScrapingMetricsAggregator.Items.GroupBy(Grouping))
            {
                FillDatarowForTimingMetric(dt, $"({scenarioGroup.Select(x=> x.Url).Distinct().Count()} urls) {scenarioGroup.Key}", scenarioGroup, _criteria);
            }

            var output = AsciiTableGenerator.CreateAsciiTableFromDataTable(dt);
            Console.Write(output);
        }
    }
}
