﻿using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.WebScrapingStrategies.CDP
{
    public class CdpScraperStrategy : IWebScraperStrategy
    {
        public string ScraperName { get; set; }

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

        string IWebScraperStrategy.GetCleanedHtml()
        {
            throw new NotImplementedException();
        }
    }
}