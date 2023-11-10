using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebscrapingBenchmark.BaselineExecutor.WebScrapingStrategies;
using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.Interfaces;

namespace WebscrapingBenchmark.BaselineExecutor.Installers
{
    public class BaselineExecutor : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IWebScraperStrategy>().ImplementedBy<RealBaseline>().Named("ActualBaseline").LifestyleTransient(),
                Component.For<ICacheWarmer>().ImplementedBy<FilesystemRequestCacheWarmer>()
                );
        }
    }
}
