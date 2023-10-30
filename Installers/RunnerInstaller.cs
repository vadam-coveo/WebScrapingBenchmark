using System.ComponentModel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.ScenarioRunner;
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
            foreach (var scenario in Scenarios)
            {
                RegisterScenario(scenario, container);
            }
        }

        private void RegisterScenario(ConfigurationScenario scenario, IWindsorContainer container)
        {
            container.Register(Component.For<ConfigurationScenario>().Instance(scenario).Named(scenario.ScenarioName));
            container.Register(Component.For<IScenarioRunner>().ImplementedBy<ScenarioRunner>().DependsOn(Dependency.OnComponent(typeof(ConfigurationScenario), scenario.ScenarioName)));
        }
    }
}
