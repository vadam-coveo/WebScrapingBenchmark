using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebscrapingBenchmark.Core.Framework.Config;
using WebscrapingBenchmark.Core.Framework.Interfaces;
using WebscrapingBenchmark.Core.Framework.ScenarioRunner;
using Component = Castle.MicroKernel.Registration.Component;

namespace WebscrapingBenchmark.Core.Installers
{
    /// <summary>
    /// Must be installed last because it depends on previously installed Configs and Scraper strategies
    /// </summary>
    public class RunnerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var scenarioNames = GetAllRegisteredComponentsForTypeSelector(container, typeof(ConfigurationScenario));
            var scrapingStrategies = GetAllRegisteredComponentsForTypeSelector(container, typeof(IWebScraperStrategy));

            RegisterRunners(container, scenarioNames, scrapingStrategies);
        }

        private void RegisterRunners(IWindsorContainer container, IEnumerable<string> scenarioComponentNames, IEnumerable<string> scrapingStrategies)
        {
            foreach (var scenario in scenarioComponentNames)
            {
                foreach (var strategy in scrapingStrategies)
                {
                    container.Register(Component.For<IScenarioRunner>().ImplementedBy<ScenarioRunner>()
                        .LifestyleTransient()
                        .DependsOn(Dependency.OnComponent(typeof(ConfigurationScenario), scenario), Dependency.OnComponent(typeof(IWebScraperStrategy), strategy))
                        .Named($"{strategy} - {scenario}")
                    );
                }
            }
        }

        private List<string> GetAllRegisteredComponentsForTypeSelector(IWindsorContainer container, Type type)
        {
            return container.Kernel.GetHandlers(type).Select(h => h.ComponentModel.Name).ToList();
        }
    }
}
