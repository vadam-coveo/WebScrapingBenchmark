using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class BenchmarkAggregator : IBenchmarkAggregator
    {
        private readonly ConcurrentBag<Benchmark> _benchmarks = new ConcurrentBag<Benchmark>();

        public void AddBenchmark(Benchmark benchmark)
        {
            _benchmarks.Add(benchmark);
        }

        public void ReportBenchmarks()
        {
            Console.WriteLine("Results:");

            foreach (var benchmark in _benchmarks)
            {
                var builder = new StringBuilder();
                builder.AppendLine($"ScenarioName: {benchmark.ScenarioName}");
                builder.AppendLine($"ScraperName: {benchmark.ScraperName}");
                builder.AppendLine($"Nb of URLs: {benchmark.BenchmarkPerUrl.Count}");

                builder.AppendLine($"Average GoToUrl: {benchmark.BenchmarkPerUrl.Select(url => url.GoToUrlTiming).Average()}");
                builder.AppendLine($"Average Load: {benchmark.BenchmarkPerUrl.Select(url => url.LoadTiming).Average()}");
                builder.AppendLine($"Average GetHtmlResult: {benchmark.BenchmarkPerUrl.Select(url => url.GetHtmlResultTiming).Average()}");

                builder.AppendLine($"Average MetadataExtraction: {benchmark.BenchmarkPerUrl.SelectMany(url => url.MetadataExtractionTiming.Select(timing => timing.Duration)).Average()}");
                builder.AppendLine($"Average ContentExclusion: {benchmark.BenchmarkPerUrl.SelectMany(url => url.ContentExclusionTiming.Select(timing => timing.Duration)).Average()}");

                Console.WriteLine(builder);
            }
        }
    }
}
