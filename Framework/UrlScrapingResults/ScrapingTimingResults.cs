using WebScrapingBenchmark.Framework.ScenarioRunner;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults;

public class ScrapingTimingResults : BaseUrlScrapingResult
{
    public TimeSpan GoToUrlTiming;
    public TimeSpan LoadTiming;
    public TimeSpan GetHtmlResultTiming;

    public List<ElementTiming> MetadataExtractionTiming = new();
    public List<ElementTiming> ContentExclusionTiming = new();

    public ScrapingTimingResults(string url, string scenarioName, string scraperName) : base(url, scenarioName, scraperName)
    {
    }
}