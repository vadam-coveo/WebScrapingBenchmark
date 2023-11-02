using CsvHelper.Configuration.Attributes;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults
{
    public abstract class BaseUrlScrapingResult
    {
        [Ignore]
        public string Url { get; }

        [Ignore]
        
        public string ScenarioName { get; }

        [Index(1)]
        public  string ScraperName { get; }

        [Index(0)]
        public string ScenarioIdentifier => $"[{ScenarioName}] : {Url}";

        [Ignore]
        public string UniqueIdentifier => $"[{ScenarioName}-{ScraperName}] : {Url} ";

        public BaseUrlScrapingResult(string url, string scenarioName, string scraperName)
        {
            Url = url;
            ScenarioName = scenarioName;
            ScraperName = scraperName;
        }
    }
}
