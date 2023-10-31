﻿using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ChromedriverWrapper;

namespace WebScrapingBenchmark.Installers
{
    public class FrameworkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, allowEmptyCollections: false));
            container.AddFacility<TypedFactoryFacility>();

            container.Register(Component.For<IChromeDriverWrapper>().ImplementedBy<ChromeDriverWrapper>()); // we'll try with 1 singleton instance for now since we're not running anything in parallel
        }
    }
}
