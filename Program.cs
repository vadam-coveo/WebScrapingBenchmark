

using Castle.Windsor;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.Logging;
using WebScrapingBenchmark.Framework.ScenarioRunner;
using WebScrapingBenchmark.Installers;
using WebScrapingBenchmark.WebScrapingStrategies;

// todo : maybe later, we may want to have the filter as a cli argument?
// todo : we may also want to set minimum loglevel?

var scenarios = ScenarioLoader.LoadScenarios(Directory.GetCurrentDirectory() + "\\..\\..\\..\\TestCaseData", "*.json");


var container = new WindsorContainer();
container.Install(new FrameworkInstaller(),
                    new RunnerInstaller(scenarios)
                    );

var runners = container.ResolveAll<IScenarioRunner>().OrderBy(x=> x.WebScraper.GetType().Name).ToList();

var previousScraper = (IWebScraperStrategy)null;

foreach (var runner in runners)
{
    try
    {
        if (previousScraper == null || previousScraper != runner.WebScraper)
        {
            previousScraper = runner.WebScraper;
            ConsoleLogger.Warn($"\r\r-----------------Scraper: {runner.WebScraper.GetType().Name}-----------------");
        }

        runner.RunScenario();
    }
    finally
    {
        container.Release(runner);
    }
}

var benchmarkAggregator = container.Resolve<IBenchmarkAggregator>();
benchmarkAggregator.ReportBenchmarks();

// todo : interpret metrics

container.Dispose();

// to allow chromedrivers to actually get killed
Thread.Sleep(TimeSpan.FromSeconds(30));