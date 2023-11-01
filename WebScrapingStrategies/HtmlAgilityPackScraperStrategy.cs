using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.HtmlProcessors;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public class HtmlAgilityPackScraperStrategy : HtmlProcessorBase
    {
        public HtmlAgilityPackScraperStrategy(IChromeDriverWrapper driverWrapper, IHtmlProcessorFactory htmlProcessorFactory) : base(driverWrapper, htmlProcessorFactory)
        {
        }

        protected override IHtmlProcessor CreateHtmlProcessorInstance()
        {
            return HtmlProcessorFactory.GetHtmlAgilityPackHtmlProcessor(HtmlBody);
        }
    }
}
