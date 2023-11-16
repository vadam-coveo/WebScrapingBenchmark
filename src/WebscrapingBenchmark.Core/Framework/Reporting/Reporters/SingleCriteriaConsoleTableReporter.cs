using WebscrapingBenchmark.Core.Framework.Logging;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Framework.Reporting.Reporters
{
    public class SingleCriteriaConsoleTableReporter : ConsoleTableResultReporter
    {
        private Func<ScrapingMetrics, TimeSpan> Criteria { get; }

        private AggregatingMethod AggregatingMethod { get; }

        public SingleCriteriaConsoleTableReporter(int index, string reporterName, Func<ScrapingMetrics, string> grouping, Func<ScrapingMetrics, TimeSpan> criteria, IAggregator<ScrapingMetrics> scrapingMetricsAggregator, AggregatingMethod aggregatingMethod) 
            : base(index, reporterName, grouping, scrapingMetricsAggregator)
        {
            Criteria = criteria;
            AggregatingMethod = aggregatingMethod;
        }

        public override void ReportResults()
        {
            ConsoleLogger.Info("\r\r");
            ConsoleLogger.Info("\r\r");
            ConsoleLogger.Warn($"--------------------------------{ReporterName}---------------------------");

            
            var dt = InitDataTable(ScrapingMetricsAggregator.Items, "Group");
            
            foreach (var scenarioGroup in ScrapingMetricsAggregator.Items.GroupBy(Grouping))
            {
                FillDatarowForTimingMetric(dt, $"({scenarioGroup.Select(x=> x.Url).Distinct().Count()} urls) {scenarioGroup.Key}", scenarioGroup, Criteria, AggregatingMethod);
            }

            var output = AsciiTableGenerator.CreateAsciiTableFromDataTable(dt);
            Console.Write(output);
        }
    }
}
