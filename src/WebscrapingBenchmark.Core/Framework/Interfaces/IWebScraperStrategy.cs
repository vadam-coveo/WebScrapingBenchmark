using Coveo.Connectors.Utilities.Web.WebScraping;

namespace WebscrapingBenchmark.Core.Framework.Interfaces
{
    public interface IWebScraperStrategy
    {
        /// <summary>
        /// Informs the executor that requests are cached, therefore request timing should be added to the load time
        /// </summary>
        public bool UsesCachedRequests { get; }

        /// <summary>
        /// Method to get to the desired url
        /// </summary>
        /// <param name="url"></param>
        public string GoToUrl(string url);

        /// <summary>
        /// Method to load the libraries for scraping, if needed
        /// </summary>
        public void Load();

        /// <summary>
        /// Method to exclude a specific section of html
        /// </summary>
        /// <param name="selector"></param>
        public bool ExcludeHtml(Selector selector);

        /// <summary>
        /// Method to extract metadata by selector
        /// </summary>
        /// <param name="selector"></param>
        public IEnumerable<string?> ExtractMetadata(Selector selector);

        /// <summary>
        /// Method to provide the end-result html
        /// </summary>
        public string GetCleanedHtml();
    }
}
