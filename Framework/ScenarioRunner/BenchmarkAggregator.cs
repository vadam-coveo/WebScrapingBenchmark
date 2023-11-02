using System.Collections.Concurrent;
using System.Text;
using Humanizer;
using WebScrapingBenchmark.Framework.ScrapingResultComparing;

namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class BenchmarkAggregator : IBenchmarkAggregator
    {
        private readonly ConcurrentBag<Benchmark> _benchmarks = new ConcurrentBag<Benchmark>();
        private readonly ConcurrentBag<ScrapingOutput> _scrapingOutputs = new();

        public void AddBenchmark(Benchmark benchmark)
        {
            _benchmarks.Add(benchmark);
        }

        public void ReportBenchmarks()
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("ScenarioName,ScraperName,URL,GoToUrlTiming,GoToUrlDiffWithAvg,GoToUrlDiffWithFastest,LoadTiming,LoadDiffWithAvg,LoadDiffWithFastest,GetHtmlResultTiming,GetHtmlResultDiffWithAvg,GetHtmlResultDiffWithFastest");

            Console.WriteLine("Results:");

            foreach (var benchmark in _benchmarks)
            {
                var builder = new StringBuilder();
                builder.AppendLine($"ScenarioName: {benchmark.ScenarioName}");
                builder.AppendLine($"ScraperName: {benchmark.ScraperName}");
                builder.AppendLine($"Nb of URLs: {benchmark.BenchmarkPerUrl.Count}");

                builder.AppendLine($"Average GoToUrl: {benchmark.AverageGoToUrl.Value.Humanize(3)}");
                builder.AppendLine($"Average Load: {benchmark.AverageLoad.Value.Humanize(3)}");
                builder.AppendLine($"Average GetHtmlResult: {benchmark.AverageGetHtmlResult.Value.Humanize(3)}");

                builder.AppendLine($"Average MetadataExtraction: {benchmark.AverageMetadataExtraction.Value.Humanize(3)}");
                builder.AppendLine($"Average ContentExclusion: {benchmark.AverageContentExclusion.Value.Humanize(3)}");

                Console.WriteLine(builder);

                foreach (var url in benchmark.BenchmarkPerUrl)
                {
                    csvBuilder.AppendLine(
                        $"{benchmark.ScenarioName},{benchmark.ScraperName},{url.Url}," +
                        $"{url.GoToUrlTiming},{url.GoToUrlTiming - benchmark.AverageGoToUrl.Value},{url.GoToUrlTiming - benchmark.FastestGoToUrl.Value}," +
                        $"{url.LoadTiming},{url.LoadTiming - benchmark.AverageLoad.Value},{url.LoadTiming - benchmark.FastestLoad.Value}," +
                        $"{url.GetHtmlResultTiming},{url.GetHtmlResultTiming - benchmark.AverageGetHtmlResult.Value},{url.GetHtmlResultTiming - benchmark.FastestGetHtmlResult.Value}");
                }
            }

            File.AppendAllText("result.csv", csvBuilder.ToString());
        }
    }
}
