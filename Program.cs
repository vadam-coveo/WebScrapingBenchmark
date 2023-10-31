
using Castle.Windsor;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.ScenarioRunner;
using WebScrapingBenchmark.Installers;

// todo : maybe later, we may want to have the filter as a cli argument?
// todo : we may also want to set minimum loglevel?

var scenarios = ScenarioLoader.LoadScenarios(Directory.GetCurrentDirectory() + "\\..\\..\\..\\TestCaseData", "*.json");


var container = new WindsorContainer();
container.Install(new FrameworkInstaller(),
                    new RunnerInstaller(scenarios)
                    );

var runners = container.ResolveAll<IScenarioRunner>();

foreach (var runner in runners)
{
    try
    {
        runner.RunScenario();
    }
    finally
    {
        container.Release(runner);
    }
}

// todo : interpret metrics

container.Dispose();

// to allow chromedrivers to actually get killed
Thread.Sleep(TimeSpan.FromSeconds(30));