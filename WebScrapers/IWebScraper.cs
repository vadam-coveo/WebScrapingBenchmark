using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.WebScrapers
{
    public interface IWebScraper
    {
        public string ScraperName { get; set; }

        public string GetCleanedHtml { get; }

        public void GoToUrl(string url);

        public void ExcludeHtml(Selector selector);

        public string ExtractMetadata(Selector selector);
    }
}
