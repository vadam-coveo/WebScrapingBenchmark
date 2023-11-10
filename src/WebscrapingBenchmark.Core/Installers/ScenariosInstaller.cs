using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebscrapingBenchmark.Core.Framework.Config;
using WebscrapingBenchmark.Core.Framework.Helpers;

namespace WebscrapingBenchmark.Core.Installers
{
    public class ScenariosInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterScenarios(container);
        }

        /// <summary>
        /// Registers all scenarios, returns all their component names
        /// </summary>
        private void RegisterScenarios(IWindsorContainer container)
        {
            var files = Directory.GetFiles(FilesystemHelper.Solution.ScenarioDirectoryPath, "*.json");

            if (!files.Any())
            {
                throw new ArgumentException(
                    $"There are no scenario files located in {FilesystemHelper.Solution.ScenarioDirectoryPath}");
            }

            foreach (var file in files)
            {
                var scenario = FilesystemHelper.FromJsonFile<ConfigurationScenario>(file);
                container.Register(Component.For<ConfigurationScenario>().Instance(scenario).Named(scenario.ScenarioName));
            }
        }
    }
}
