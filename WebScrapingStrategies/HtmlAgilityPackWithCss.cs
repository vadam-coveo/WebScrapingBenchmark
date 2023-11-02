using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.HtmlProcessors;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public class HtmlAgilityPackWithCss : HtmlProcessorBase
    {
        public HtmlAgilityPackWithCss(IChromeDriverWrapper driverWrapper, IHtmlProcessorFactory htmlProcessorFactory) : base(driverWrapper, htmlProcessorFactory)
        {
        }

        protected override IHtmlProcessor CreateHtmlProcessorInstance()
        {
            return HtmlProcessorFactory.GetHtmlAgilityPackHtmlProcessor(HtmlBody, true);
        }
    }
}
