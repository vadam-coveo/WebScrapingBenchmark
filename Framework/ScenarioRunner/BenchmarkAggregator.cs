using System.Collections.Concurrent;

namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class BenchmarkAggregator : IBenchmarkAggregator
    {
        private ConcurrentBag<Benchmark> _benchmarks = new ConcurrentBag<Benchmark>();

        public void AddBenchmark(Benchmark benchmark)
        {
            _benchmarks.Add(benchmark);
        }
    }
}
