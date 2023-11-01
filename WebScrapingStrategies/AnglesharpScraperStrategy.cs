using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public class AnglesharpScraperStrategy : IWebScraperStrategy
    {
        public string ScraperName { get; set; }

        public void GoToUrl(string url)
        {
        }

        public void Load()
        {
    
        }

        public bool ExcludeHtml(Selector selector)
        {
            return false;
        }

        public IEnumerable<string> ExtractMetadata(Selector selector)
        {
            return Enumerable.Empty<string>();
        }

        public string GetCleanedHtml()
        {
            return null;
        }
    }
}
