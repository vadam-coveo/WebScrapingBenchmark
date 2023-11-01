using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebScrapingBenchmark.WebScrapingStrategies;
using WebScrapingBenchmark.WebScrapingStrategies.Anglesharp;

namespace WebScrapingBenchmark.Installers
{
    public class ScrapersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterAnglesharp(container);
        }

        private void RegisterAnglesharp(IWindsorContainer container)
        {
            container.Register(Component.For<IWebScraperStrategy>().ImplementedBy<AnglesharpScraperStrategy>().LifestyleTransient().Named(nameof(AnglesharpScraperStrategy)));
        }
    }
}
