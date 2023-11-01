using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public interface IWebScraperStrategy
    {
        /// <summary>
        /// Method to get to the desired url
        /// </summary>
        /// <param name="url"></param>
        public void GoToUrl(string url);

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
        public IEnumerable<string> ExtractMetadata(Selector selector);

        /// <summary>
        /// Method to provide the end-result html
        /// </summary>
        public string GetCleanedHtml();
    }
}
