using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.WebScrapingStrategies;

namespace WebScrapingBenchmark.Framework.ScenarioRunner;

public interface IScenarioRunner
{
    IWebScraperStrategy WebScraper { get; }
    ConfigurationScenario Scenario { get; }
    void RunScenario();
}