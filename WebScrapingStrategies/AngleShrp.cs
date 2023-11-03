using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.HtmlProcessors;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public class AngleShrp : HtmlProcessorBase
    {
        public AngleShrp(IChromeDriverWrapper driverWrapper, IHtmlProcessorFactory htmlProcessorFactory) : base(driverWrapper, htmlProcessorFactory)
        {
        }

        protected override IHtmlProcessor CreateHtmlProcessorInstance()
        {
            return HtmlProcessorFactory.GetAngleSharpHtmlProcessor(HtmlBody, true);
        }
    }
}
