using Castle.Windsor;
using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.Reporting;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Installers;
using WebscrapingBenchmark.NewStrategiesExecutor.Installers;


var container = new WindsorContainer();
container.Install(
    new ScenariosInstaller(),
    new FrameworkInstaller(),
    new NewScrapersInstaller(),
    new RunnerInstaller()
);

var cacheWarmers = container.ResolveAll<ICacheWarmer>();
foreach (var cacheWarmer in cacheWarmers)
{
    cacheWarmer.WarmCache();
    container.Release(cacheWarmer);
}

var runnerAggregator = container.Resolve<IScenarioRunnerAggregator>();
runnerAggregator.RunAllScenarios();
container.Release(runnerAggregator);

var resultAggregator = container.Resolve<IScrapingResultReportingAggregator>();
resultAggregator.Run();
container.Release(resultAggregator);

container.Dispose();