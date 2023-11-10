using System.Collections.Concurrent;

namespace WebscrapingBenchmark.Core.Framework.ScenarioRunner
{
    public interface IAggregator<T> where T : class
    {
        IEnumerable<T> Items { get; }
        void Aggregate(T input);
    }

    public class ConcurrentAggregator<T> : IAggregator<T> where T : class
    {
        private readonly ConcurrentBag<T> _collection = new ConcurrentBag<T>();
        public IEnumerable<T> Items => _collection;

        public void Aggregate(T input)
        {
            _collection.Add(input);
        }
    }
}
