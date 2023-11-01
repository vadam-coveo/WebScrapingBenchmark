using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.HtmlProcessors;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public class AngleSharpScraperStrategy : HtmlProcessorBase
    {
        public AngleSharpScraperStrategy(IChromeDriverWrapper driverWrapper, IHtmlProcessorFactory htmlProcessorFactory) : base(driverWrapper, htmlProcessorFactory)
        {
        }

        protected override IHtmlProcessor CreateHtmlProcessorInstance()
        {
            return HtmlProcessorFactory.GetAngleSharpHtmlProcessor(HtmlBody);
        }
    }
}
