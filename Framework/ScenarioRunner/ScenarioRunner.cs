using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.WebScrapers;

namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class ScenarioRunner : IScenarioRunner
    {
        public ScenarioRunner(IWebScraper[] webScrapers, ConfigurationScenario scenario)
        {
            WebScrapers = webScrapers;
            Scenario = scenario;
        }

        public IWebScraper[] WebScrapers { get; }
        public ConfigurationScenario Scenario { get; } // todo : maybe store in state the metrics we might want to keep

        public void RunScenario()
        {
            foreach (var scraper in WebScrapers)
            {
                // todo
            }
        }

    }
}
