using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.WebScrapers.CDP
{
    public class CdpScraper : IWebScraper
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
