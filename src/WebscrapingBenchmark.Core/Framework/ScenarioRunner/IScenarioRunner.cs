using WebscrapingBenchmark.Core.Framework.Config;
using WebscrapingBenchmark.Core.Framework.Interfaces;

namespace WebscrapingBenchmark.Core.Framework.ScenarioRunner;

public interface IScenarioRunner
{
    IWebScraperStrategy WebScraper { get; }
    ConfigurationScenario Scenario { get; }
    void RunScenario();
}