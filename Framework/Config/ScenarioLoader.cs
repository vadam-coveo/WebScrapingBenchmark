using Newtonsoft.Json;

namespace WebScrapingBenchmark.Framework.Config
{
    public static class ScenarioLoader
    {
        public static List<ConfigurationScenario> LoadScenarios(string path, string searchPattern = "*.json")
        {
            var files = Directory.GetFiles(path, "*.json");

            var scenarios = new List<ConfigurationScenario>();

            foreach (var file in files)
            {
                var scenario = JsonConvert.DeserializeObject<ConfigurationScenario>(File.OpenText(file).ReadToEnd());
                scenarios.Add(scenario);
            }

            return scenarios;
        }
    }
}
