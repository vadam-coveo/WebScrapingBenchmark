using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.ChromeDriver;
using WebscrapingBenchmark.Core.Framework.Reporting;
using WebscrapingBenchmark.Core.Framework.Reporting.Reporters;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Installers
{
    public class FrameworkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, allowEmptyCollections: false));
            container.AddFacility<TypedFactoryFacility>();

            container.Register(Component.For<IChromeDriverWrapper>().ImplementedBy<ChromeDriverWrapper>().LifestyleTransient(), 
                                Component.For<IChromeDriverWrapperFactory>().AsFactory(),
                                Component.For<ICache<CachedRequest>>().ImplementedBy<Cache<CachedRequest>>(),
                                Component.For<IScenarioRunnerAggregator>().ImplementedBy<ScenarioRunnerAggregator>(),
                                Component.For<IScrapingResultReportingAggregator>().ImplementedBy<ScrapingResultReportingAggregator>(),
                                Component.For<IScenarioRunnerFactory>().AsFactory(),
                                Component.For<IReporterFactory>().AsFactory()
            );


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
                    .DependsOn(Dependency.OnValue<int>(3)),

                Component.For<IScrapingResultsReporter>().ImplementedBy<FilesystemJsonReporter>().DependsOn(Dependency.OnValue<int>(4))
            );
        }
    }
}
