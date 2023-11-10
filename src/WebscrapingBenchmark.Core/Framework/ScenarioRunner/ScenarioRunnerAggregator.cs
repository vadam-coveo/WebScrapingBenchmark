using WebscrapingBenchmark.Core.Framework.Logging;

namespace WebscrapingBenchmark.Core.Framework.ScenarioRunner
{
    public class ScenarioRunnerAggregator : IScenarioRunnerAggregator
    {
        private IScenarioRunnerFactory ScenarioRunnerFactory { get; }

        public ScenarioRunnerAggregator(IScenarioRunnerFactory scenarioRunnerFactory)
        {
            ScenarioRunnerFactory = scenarioRunnerFactory;
        }
        
        public void RunAllScenarios()
        {
            var runners = ScenarioRunnerFactory.CreateAll().OrderBy(x => x.WebScraper.GetType().Name).ToList();

            var previousScraper = "";

            foreach (var runner in runners)
            {
                try
                {
                    if (previousScraper == null || previousScraper != runner.WebScraper.GetType().Name)
                    {
                        previousScraper = runner.WebScraper.GetType().Name;
                        ConsoleLogger.Warn($"\r\r-----------------Scraper: {runner.WebScraper.GetType().Name}-----------------");
                    }

                    runner.RunScenario();
                }
                finally
                {
                    ScenarioRunnerFactory.Release(runner);
                }
            }
        }
    }
}
