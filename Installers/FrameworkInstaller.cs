﻿using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.Reporting;
using WebScrapingBenchmark.Framework.Reporting.Reporters;
using WebScrapingBenchmark.Framework.ScenarioRunner;
using WebScrapingBenchmark.Framework.UrlScrapingResults;

namespace WebScrapingBenchmark.Installers
{
    public class FrameworkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, allowEmptyCollections: false));
            container.AddFacility<TypedFactoryFacility>();

            container.Register(Component.For<IChromeDriverWrapper>().ImplementedBy<ChromeDriverWrapper>()); // we'll try with 1 singleton instance for now since we're not running anything in parallel
            container.Register(Component.For<ICache<CachedRequest>>().ImplementedBy<Cache<CachedRequest>>());


            RegisterAggregators(container);
            RegisterReporters(container);
        }
        
        private void RegisterAggregators(IWindsorContainer container)
        {
            container.Register(
                Component.For<IAggregator<ScrapingMetrics>>().ImplementedBy<ConcurrentAggregator<ScrapingMetrics>>()
            );
        }
        private void RegisterReporters(IWindsorContainer container)
        {
            container.Register(
                Component.For<IScrapingResultsReporter>().ImplementedBy<ConsoleTableResultReporter>()
                    .DependsOn(
                        Dependency.OnValue<int>(1), 
                        Dependency.OnValue<string>("Compounded Results Per URL"),
                        Dependency.OnValue<Func<ScrapingMetrics, string>>((ScrapingMetrics metric) => metric.ScenarioIdentifier))
                    .Named($"{nameof(ConsoleTableResultReporter)}-PerUrl"),

                Component.For<IScrapingResultsReporter>().ImplementedBy<ConsoleTableResultReporter>()
                    .DependsOn(
                        Dependency.OnValue<int>(2), 
                        Dependency.OnValue<string>("Compounded Results Per Scenario"),
                        Dependency.OnValue<Func<ScrapingMetrics, string>>((ScrapingMetrics metric) => metric.ScenarioName))
                    .Named($"{nameof(ConsoleTableResultReporter)}-PerScenario"),

                Component.For<IScrapingResultsReporter>().ImplementedBy<CsvResultReporter>()
                    .DependsOn(Dependency.OnValue<int>(3))
            );
        }
    }
}
