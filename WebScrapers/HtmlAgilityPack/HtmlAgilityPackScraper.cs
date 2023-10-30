using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.WebScrapers.HtmlAgilityPack
{
    public class HtmlAgilityPackScraper : IWebScraper
    {
        public string ScraperName { get; set; }
        public string GetCleanedHtml { get; }

        public void GoToUrl(string url)
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
