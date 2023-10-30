using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebScrapingBenchmark.WebScrapers;
using WebScrapingBenchmark.WebScrapers.Anglesharp;

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
            container.Register(Component.For<IWebScraper>().ImplementedBy<AnglesharpScraper>());

        }

        // todo : the rest
    }
}
