using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.WebScrapingStrategies.HtmlAgilityPack
{
    public class HtmlAgilityPackScraperStrategy : IWebScraperStrategy
    {
        public string ScraperName { get; set; }

        public string GetCleanedHtml()
        {
            throw new NotImplementedException();
        }

        public void GoToUrl(string url)
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void ExcludeHtml(Selector selector)
        {
            throw new NotImplementedException();
        }

        public string ExtractMetadata(Selector selector)
        {
            throw new NotImplementedException();
        }
    }
}
