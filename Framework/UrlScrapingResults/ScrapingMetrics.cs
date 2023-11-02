using WebScrapingBenchmark.Framework.ScrapingResultComparing;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults
{
    public class ScrapingMetrics : ScrapingOutput
    {
        public ScrapingMetrics(string url, string scenarioName, string scraperName) : base(url, scenarioName, scraperName)
        {
        }
    }
}
