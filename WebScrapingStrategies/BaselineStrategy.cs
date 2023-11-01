using OpenQA.Selenium.Chrome;
using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.HtmlProcessors;
using WebScrapingBenchmark.Framework.Logging;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public class BaselineStrategy : IWebScraperStrategy, IDisposable
    {
        private IChromeDriverWrapper DriverWrapper { get; }
        private IHtmlProcessorFactory HtmlProcessorFactory { get; }

        private ChromeDriver Driver => DriverWrapper.Driver;

        private string htmlbody;

        private Lazy<IHtmlProcessor> AnglesharpProcessor; 

        public BaselineStrategy(IChromeDriverWrapper driverWrapper, IHtmlProcessorFactory htmlProcessorFactory)
        {
            DriverWrapper = driverWrapper;
            HtmlProcessorFactory = htmlProcessorFactory;
        }

        public void GoToUrl(string url)
        {
            htmlbody = DriverWrapper.GetHtml(url, TimeSpan.FromSeconds(2));
            ResetAnglesharp(htmlbody);
        }

        public void Load()
        {
        }

        public void ExcludeHtml(Selector selector)
        {
            var excluded = AnglesharpProcessor.Value.Remove(selector);
            ConsoleLogger.Debug($"          Excluded = {excluded} for selector {selector.Path}");
        }

        public string ExtractMetadata(Selector selector)
        {
            var result = AnglesharpProcessor.Value.Extract(selector);
            ConsoleLogger.Debug($"          Extracted = {result.Count()} for selector {selector.Path}");
            return null;
        }

        public string GetCleanedHtml()
        {
            return AnglesharpProcessor.Value.GetHtmlBody();
        }

        public void Dispose()
        {
            if (AnglesharpProcessor.IsValueCreated)
            {
                HtmlProcessorFactory.Release(AnglesharpProcessor.Value);
            }
        }

        private void ResetAnglesharp(string html)
        {
            if (AnglesharpProcessor?.IsValueCreated ?? false)
            {
                HtmlProcessorFactory.Release(AnglesharpProcessor.Value);
            }

            AnglesharpProcessor = new Lazy<IHtmlProcessor>(() => HtmlProcessorFactory.CreateAnglesharpHtmlProcessor(html));
        }
    }
}
