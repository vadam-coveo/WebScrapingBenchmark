using System.Reflection;
using Newtonsoft.Json;

namespace WebscrapingBenchmark.Core.Framework.Helpers
{
    public static class FilesystemHelper
    {
        public static T FromJsonFile<T>(string filepath)
        {
            var jsonString = File.ReadAllText(filepath);
            return JsonConvert.DeserializeObject<T>(jsonString)!;
        }

        public static void ToJsonFile<T>(T item, string filepath)
        {
            var jsonString = JsonConvert.SerializeObject(item, Formatting.Indented);

            if (File.Exists(filepath)) File.Delete(filepath);

            File.WriteAllText(filepath, jsonString);
        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        public static SolutionExplorer Solution => _solution ??= new SolutionExplorer();
        private static SolutionExplorer? _solution;

        public class SolutionExplorer
        {
            public string RootPath { get; private set; }
            public string WorkFolderPath { get; private set; }
            public string MetricsOutputDirectory { get; private set; }
            public string RequestCacheDirectory { get; private set; }
            public string BaselineExecutorPath { get; private set; }
            public string NewStrategiesExecutorPath { get; private set; }
            public string CsvOutputDirectory { get; private set; }
            public string ScenarioDirectoryPath { get; private set; }
            public string ChromiumPath { get; private set; }

            public SolutionExplorer()
            {
                RootPath = GetRootPath();
                WorkFolderPath = Path.Combine(RootPath, "WorkFolder");

                MetricsOutputDirectory = Path.Combine(WorkFolderPath, "Metrics");
                RequestCacheDirectory = Path.Combine(WorkFolderPath, "Requests");
                ScenarioDirectoryPath = Path.Combine(WorkFolderPath, "Scenarios");
                CsvOutputDirectory = Path.Combine(WorkFolderPath, "Csv");

                BaselineExecutorPath = GetExecutablePath("WebscrapingBenchmark.BaselineExecutor");
                NewStrategiesExecutorPath = GetExecutablePath("WebscrapingBenchmark.NewStrategiesExecutor");
                ChromiumPath = Path.Combine(RootPath, "Chromium", "Application", "chrome.exe");
                CreateRequiredDirectories();
            }

            private void CreateRequiredDirectories()
            {
                SafeCreateDirectory(WorkFolderPath);
                SafeCreateDirectory(MetricsOutputDirectory);
                SafeCreateDirectory(RequestCacheDirectory);
                SafeCreateDirectory(ScenarioDirectoryPath);
                SafeCreateDirectory(CsvOutputDirectory);
            }

            private void SafeCreateDirectory(string path)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            private string GetExecutablePath(string projectName)
            {
                return Path.Combine(RootPath, "src", projectName, "bin", "Debug", "net6.0", $"{projectName}.exe");
            }

            private string GetRootPath()
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                var current = new DirectoryInfo(path);

                while (true)
                {
                    if (current.GetFiles("WebScrapingBenchmark.sln").Any())
                    {
                        return current.FullName;
                    }

                    current = current.Parent;
                }
            }
        }
    }
}
