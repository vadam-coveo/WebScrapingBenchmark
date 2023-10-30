
using Castle.Windsor;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.ScenarioRunner;
using WebScrapingBenchmark.Installers;

// todo : maybe later, we may want to have the filter as a cli argument?
// todo : we may also want to set minimum loglevel?

var scenarios = ScenarioLoader.LoadScenarios(Directory.GetCurrentDirectory() + "\\..\\..\\..\\TestCaseData", "*.json");


var container = new WindsorContainer();
container.Install(new FrameworkInstaller(),
                    new RunnerInstaller(scenarios), 
                    new ScrapersInstaller()
                    );

var runners = container.ResolveAll<IScenarioRunner>();

foreach (var runner in runners)
{
    runner.RunScenario();
}
