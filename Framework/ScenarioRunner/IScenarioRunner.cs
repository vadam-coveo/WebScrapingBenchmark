using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.WebScrapers;

namespace WebScrapingBenchmark.Framework.ScenarioRunner;

public interface IScenarioRunner
{
    IWebScraperStrategy WebScraper { get; }
    ConfigurationScenario Scenario { get; }
    void RunScenario();
}