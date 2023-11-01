using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.HtmlProcessors;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public class BaselineStrategy : IWebScraperStrategy, IDisposable
    {
        private IChromeDriverWrapper DriverWrapper { get; }
        private IHtmlProcessorFactory HtmlProcessorFactory { get; }

        private string _htmlBody;

        private Lazy<IHtmlProcessor> AnglesharpProcessor;
        private Lazy<IHtmlProcessor> HtmlAgilityPackHtmlProcessor;

        public BaselineStrategy(IChromeDriverWrapper driverWrapper, IHtmlProcessorFactory htmlProcessorFactory)
        {
            DriverWrapper = driverWrapper;
            HtmlProcessorFactory = htmlProcessorFactory;
        }

        public void GoToUrl(string url)
        {
            _htmlBody = DriverWrapper.GetHtml(url, TimeSpan.FromSeconds(2));
            ResetAnglesharp();
            ResetAgilityPack();
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

            HtmlAgilityPackHtmlProcessor = new Lazy<IHtmlProcessor>(() => HtmlProcessorFactory.GetHtmlAgilityPackHtmlProcessor(_htmlBody));
        }
    }
}
