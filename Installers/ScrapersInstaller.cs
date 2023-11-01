using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebScrapingBenchmark.Framework.HtmlProcessors;
using WebScrapingBenchmark.WebScrapingStrategies;
using Component = Castle.MicroKernel.Registration.Component;

namespace WebScrapingBenchmark.Installers
{
    public class ScrapersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterStrategies(container);
            RegisterHtmlProcessors(container);
        }

        private void RegisterStrategies(IWindsorContainer container)
        {
            container.Register(Classes.FromAssemblyContaining<IWebScraperStrategy>()
                .InSameNamespaceAs<IWebScraperStrategy>()
                .WithServiceAllInterfaces()
                .LifestyleTransient()
            );
        }

        private void RegisterHtmlProcessors(IWindsorContainer container)
        {
            container.Register(
                Component.For<IHtmlProcessorFactory>().AsFactory(),
                Component.For<IHtmlProcessor>().ImplementedBy<AnglesharpHtmlProcessor>().Named("AngleSharpHtmlProcessor").LifestyleTransient(),
                Component.For<IHtmlProcessor>().ImplementedBy<HtmlAgilityPackHtmlProcessor>().Named("HtmlAgilityPackHtmlProcessor").LifestyleTransient()
            );
        }
    }
}
