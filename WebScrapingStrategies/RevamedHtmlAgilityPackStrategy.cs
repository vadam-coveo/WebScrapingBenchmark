using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.HtmlProcessors;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public class RevamedHtmlAgilityPackStrategy : HtmlProcessorBase
    {
        public RevamedHtmlAgilityPackStrategy(IChromeDriverWrapper driverWrapper, IHtmlProcessorFactory htmlProcessorFactory) : base(driverWrapper, htmlProcessorFactory)
        {
        }

        protected override IHtmlProcessor CreateHtmlProcessorInstance()
        {
            return HtmlProcessorFactory.GetRevampedHtmlAgilityHtmlProcessor(HtmlBody);
        }
    }
}
