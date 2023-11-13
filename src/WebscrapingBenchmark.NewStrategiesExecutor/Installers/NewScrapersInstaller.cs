using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.Interfaces;
using WebscrapingBenchmark.Core.Framework.Reporting;
using WebscrapingBenchmark.Core.Framework.Reporting.Reporters;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;
using WebscrapingBenchmark.NewStrategiesExecutor.HtmlProcessors;
using WebscrapingBenchmark.NewStrategiesExecutor.WebScrapingStrategies;
using Component = Castle.MicroKernel.Registration.Component;

namespace WebscrapingBenchmark.NewStrategiesExecutor.Installers
{
    public class NewScrapersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterStrategies(container);
            RegisterHtmlProcessors(container);
            RegisterReporters(container);

            container.Register(
                Component.For<ICacheWarmer>().ImplementedBy<FilesystemRequestCacheWarmer>(),
                Component.For<ICacheWarmer>().ImplementedBy<ResultsWarmer>().DependsOn(Dependency.OnValue<string>(FilesystemJsonReporter.GetScraperFilterFileSearchPattern("RealBaseline-*"))),
                Component.For<IScrapingResultsReporter>().ImplementedBy<CsvResultReporter>().DependsOn(Dependency.OnValue<int>(3))
            );
        }

        private void RegisterStrategies(IWindsorContainer container)
        {
            container.Register(Classes.FromAssemblyContaining<AgilityPk>()
                .InSameNamespaceAs<AgilityPk>()
                .WithServiceAllInterfaces()
                .LifestyleTransient()
            );
        }

        private void RegisterHtmlProcessors(IWindsorContainer container)
        {
            container.Register(
                Component.For<IHtmlProcessorFactory>().AsFactory(),
                Component.For<IHtmlProcessor>().ImplementedBy<AnglesharpHtmlProcessor>().Named("AngleSharpHtmlProcessor").LifestyleTransient(),
                Component.For<IHtmlProcessor>().ImplementedBy<HtmlAgilityPackHtmlProcessor>().Named("HtmlAgilityPackHtmlProcessor").LifestyleTransient(),
                Component.For<IHtmlProcessor>().ImplementedBy<RevampedHtmlAgilityHtmlProcessor>().Named("RevampedHtmlAgilityHtmlProcessor").LifestyleTransient()
            );
        }

        private void RegisterReporters(IWindsorContainer container)
        {
            container.Register(
                SummaryTable(4, "Total Metadata Extraction Time Per Scenario", (metric) => metric.TotalMetadataExtractionTime.Value),
                SummaryTable(5, "Total Content Exclusion Time Per Scenario", (metric) => metric.TotalContentExclusionTime.Value),
                SummaryTable(6, "Total Load Time Per Scenario", (metric) => metric.LoadTiming),
                SummaryTable(7, "Total Scraping Time Per Scenario", (metric) => metric.TotalScrapingTime.Value)
                );
        }

        private IRegistration SummaryTable(int index, string name, Func<ScrapingMetrics, TimeSpan> criteria)
        {
            return Component.For<IScrapingResultsReporter>().ImplementedBy<SingleCriteriaConsoleTableReporter>()
                .DependsOn(
                    Dependency.OnValue<int>(index),
                    Dependency.OnValue<string>(name),
                    Dependency.OnValue<Func<ScrapingMetrics, string>>((ScrapingMetrics metric) =>
                        metric.ScenarioName),
                    Dependency.OnValue<Func<ScrapingMetrics, TimeSpan>>(criteria))
                .Named($"{nameof(SingleCriteriaConsoleTableReporter)}-{name}");
        }
    }
}
