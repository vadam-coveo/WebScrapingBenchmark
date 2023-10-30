using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.WebScrapers;

namespace WebScrapingBenchmark.Framework.ScenarioRunner;

public interface IScenarioRunner
{
    IWebScraper[] WebScrapers { get; }
    ConfigurationScenario Scenario { get; }
    void RunScenario();
}