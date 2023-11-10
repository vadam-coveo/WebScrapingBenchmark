using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.Interfaces;
using WebscrapingBenchmark.Core.Framework.Reporting.Reporters;
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

            container.Register(
                Component.For<ICacheWarmer>().ImplementedBy<FilesystemRequestCacheWarmer>(),
                Component.For<ICacheWarmer>().ImplementedBy<ResultsWarmer>().DependsOn(Dependency.OnValue<string>(FilesystemJsonReporter.GetScraperFilterFileSearchPattern("RealBaseline")))
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
    }
}
