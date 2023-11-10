using Coveo.Connectors.Utilities.Web.WebScraping;

namespace WebscrapingBenchmark.NewStrategiesExecutor.WebScrapingStrategies
{
    public class CdpScraperStrategy //: IWebScraperStrategy
    {
        public string ScraperName { get; set; }

        public string GoToUrl(string url)
        {
            return "";
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
