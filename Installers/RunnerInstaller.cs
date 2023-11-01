using System.ComponentModel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.ScenarioRunner;
using WebScrapingBenchmark.WebScrapingStrategies;
using Component = Castle.MicroKernel.Registration.Component;

namespace WebScrapingBenchmark.Installers
{
    public class RunnerInstaller : IWindsorInstaller
    {
        private List<ConfigurationScenario> Scenarios { get; }

        public RunnerInstaller(List<ConfigurationScenario> scenarios)
        {
            Scenarios = scenarios;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Install(new ScrapersInstaller());

            var scenarioNames = RegisterScenarios(container);
            var scrapingStrategies = GetAllRegisteredScrapingStrategyComponentNames(container);

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

        /// <summary>
        /// Registers all scenarios, returns all their component names
        /// </summary>
        private List<string> RegisterScenarios(IWindsorContainer container)
        {
            foreach (var scenario in Scenarios)
            {
                container.Register(Component.For<ConfigurationScenario>().Instance(scenario).Named(scenario.ScenarioName));
            }

            return Scenarios.Select(x => x.ScenarioName).ToList();
        }

        /// <summary>
        /// Returns component names for all registered scrapingStrategies
        /// </summary>
        private List<string> GetAllRegisteredScrapingStrategyComponentNames(IWindsorContainer container)
        {
            var handlers = container.Kernel.GetHandlers(typeof(IWebScraperStrategy));
            return handlers.Select(h => h.ComponentModel.Name).ToList();
        }
    }
}
