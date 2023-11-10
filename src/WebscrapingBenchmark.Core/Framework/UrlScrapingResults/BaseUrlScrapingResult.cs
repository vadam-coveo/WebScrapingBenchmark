using CsvHelper.Configuration.Attributes;

namespace WebscrapingBenchmark.Core.Framework.UrlScrapingResults
{
    public abstract class BaseUrlScrapingResult
    {
        [Ignore]
        public string Url { get; set; }

        [Ignore]
        
        public string ScenarioName { get; set; }

        [Index(1)]
        public  string ScraperName { get; set; }

        [Index(0)]
        public string ScenarioIdentifier => $"[{ScenarioName}] : {Url}";

        [Ignore]
        public string UniqueIdentifier => $"[{ScenarioName}-{ScraperName}] : {Url} ";

        // needed for serializer
        public BaseUrlScrapingResult(){}

        public BaseUrlScrapingResult(string url, string scenarioName, string scraperName)
        {
            Url = url;
            ScenarioName = scenarioName;
            ScraperName = scraperName;
        }
    }
}
