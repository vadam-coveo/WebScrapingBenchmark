using Coveo.Connectors.Utilities.Web.WebScraping;
using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.ChromeDriver;
using WebscrapingBenchmark.Core.Framework.Interfaces;
using WebscrapingBenchmark.NewStrategiesExecutor.HtmlProcessors;

namespace WebscrapingBenchmark.NewStrategiesExecutor.WebScrapingStrategies
{
    public abstract class HtmlProcessorBase : IWebScraperStrategy, IDisposable
    {
        private ICache<CachedRequest> _requestCache;
        protected IHtmlProcessorFactory HtmlProcessorFactory { get; }
        public bool UsesCachedRequests => true;
        protected IHtmlProcessor HtmlProcessor { get; private set; }

        protected string HtmlBody;

        public HtmlProcessorBase(IHtmlProcessorFactory htmlProcessorFactory, ICache<CachedRequest> requestCache)
        {
            HtmlProcessorFactory = htmlProcessorFactory;
            _requestCache = requestCache;
        }

        protected abstract IHtmlProcessor CreateHtmlProcessorInstance();

        public string GoToUrl(string url)
        {
            var result = _requestCache.TryGet(url);
            if (result == null)
            {
                throw new Exception($"URL {url} is not loaded in cache!");
            }
            HtmlBody = result.htmlBody;
            return HtmlBody;
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
