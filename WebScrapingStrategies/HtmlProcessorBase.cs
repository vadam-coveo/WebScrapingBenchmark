using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.HtmlProcessors;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public abstract class HtmlProcessorBase : IWebScraperStrategy, IDisposable
    {
        protected IChromeDriverWrapper DriverWrapper { get; }
        protected IHtmlProcessorFactory HtmlProcessorFactory { get; }

        protected IHtmlProcessor HtmlProcessor { get; private set; }

        protected string HtmlBody;

        public HtmlProcessorBase(IChromeDriverWrapper driverWrapper, IHtmlProcessorFactory htmlProcessorFactory)
        {
            DriverWrapper = driverWrapper;
            HtmlProcessorFactory = htmlProcessorFactory;
        }

        protected abstract IHtmlProcessor CreateHtmlProcessorInstance();

        public void GoToUrl(string url)
        {
            HtmlBody = DriverWrapper.GetHtml(url, TimeSpan.FromSeconds(2));
        }

        public void Load()
        {
            HtmlProcessor = CreateHtmlProcessorInstance();
        }

        public bool ExcludeHtml(Selector selector)
        {
            return HtmlProcessor.Remove(selector);
        }

        public IEnumerable<string> ExtractMetadata(Selector selector)
        {
            return HtmlProcessor.Extract(selector);
        }

        public string GetCleanedHtml()
        {
            return HtmlProcessor.GetHtmlBody();
        }

        public void Dispose()
        {
            HtmlProcessorFactory.Release(HtmlProcessor);
        }
    }
}
