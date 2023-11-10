using Coveo.Connectors.Utilities.Web.WebScraping;
using HtmlAgilityPack;
using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.ChromeDriver;
using WebscrapingBenchmark.Core.Framework.Interfaces;
using WebscrapingBenchmark.NewStrategiesExecutor.HtmlProcessors;

namespace WebscrapingBenchmark.NewStrategiesExecutor.WebScrapingStrategies
{
    public class NewVersionBaseline : IWebScraperStrategy, IDisposable
    {
        private ICache<CachedRequest> RequestCache { get; }
        private IHtmlProcessorFactory HtmlProcessorFactory { get; }
        public bool UsesCachedRequests => true;

        private string _htmlBody;

        private Lazy<IHtmlProcessor> AnglesharpProcessor;
        private Lazy<IHtmlProcessor> HtmlAgilityPackHtmlProcessor;

        public NewVersionBaseline(IHtmlProcessorFactory htmlProcessorFactory, ICache<CachedRequest> requestCache)
        {
            RequestCache = requestCache;
            HtmlProcessorFactory = htmlProcessorFactory;
        }

        public string GoToUrl(string url)
        {
            var result = RequestCache.TryGet(url);
            if (result == null)
            {
                throw new Exception($"URL {url} is not loaded in cache!");
            }

            _htmlBody = result.htmlBody;

            ResetAnglesharp();
            ResetAgilityPack();
            return _htmlBody;
        }

        public void Load()
        {
        }

        public bool ExcludeHtml(Selector selector)
        {
            var processor = GetProcessorForSelector(selector);

            if (!processor.Remove(selector)) return false;

            _htmlBody = processor.GetHtmlBody();
            ResetProcessorThatCantProcessSelector(selector);
            return true;
        }

        public IEnumerable<string> ExtractMetadata(Selector selector)
        {
            return GetProcessorForSelector(selector).Extract(selector);
        }

        public string GetCleanedHtml()
        {
            return AnglesharpProcessor.Value.GetHtmlBody();
        }

        public void Dispose()
        {
            if (AnglesharpProcessor.IsValueCreated)
                HtmlProcessorFactory.Release(AnglesharpProcessor.Value);
            
            if (HtmlAgilityPackHtmlProcessor.IsValueCreated)
                HtmlProcessorFactory.Release(HtmlAgilityPackHtmlProcessor.Value);
        }

        private IHtmlProcessor GetProcessorForSelector(Selector selector)
        {
            if (selector.Type == SelectorType.CSS)
                return  AnglesharpProcessor.Value;

            return HtmlAgilityPackHtmlProcessor.Value;
        }

        private void ResetProcessorThatCantProcessSelector(Selector selector)
        {
            if (selector.Type == SelectorType.CSS)
            {
                ResetAgilityPack();
            }
            ResetAnglesharp();
        }

        private void ResetAnglesharp()
        {
            if (AnglesharpProcessor?.IsValueCreated ?? false)
            {
                HtmlProcessorFactory.Release(AnglesharpProcessor.Value);
            }

            AnglesharpProcessor = new Lazy<IHtmlProcessor>(() => HtmlProcessorFactory.GetAngleSharpHtmlProcessor(_htmlBody, false));
        }

        private void ResetAgilityPack()
        {
            if (HtmlAgilityPackHtmlProcessor?.IsValueCreated ?? false)
            {
                HtmlProcessorFactory.Release(HtmlAgilityPackHtmlProcessor.Value);
            }

            HtmlAgilityPackHtmlProcessor = new Lazy<IHtmlProcessor>(() => HtmlProcessorFactory.GetHtmlAgilityPackHtmlProcessor(_htmlBody, false));
        }
    }
}
