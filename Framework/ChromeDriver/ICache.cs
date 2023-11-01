namespace WebScrapingBenchmark.Framework.ChromeDriver;

public interface ICache<TType> where TType : class
{
    TType? TryGet(string key);
    TType GetOrAdd(string key, Func<string, TType> function);
    bool Contains(string key);
}