namespace WebScrapingBenchmark.Framework.UrlScrapingResults
{
    public abstract class BaseUrlScrapingResult
    {
        public readonly string Url;
        public readonly string ScenarioName;
        public readonly string ScraperName;
        public string ScenarioIdentifier => $"[{ScenarioName}] : {Url}";

        public string UniqueIdentifier => $"[{ScenarioName}-{ScraperName}] : {Url} ";

        public BaseUrlScrapingResult(string url, string scenarioName, string scraperName)
        {
            Url = url;
            ScenarioName = scenarioName;
            ScraperName = scraperName;
        }
    }
}
