using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.HtmlProcessors;

namespace WebScrapingBenchmark.WebScrapingStrategies
{
    public class AnglesharpWithXml : HtmlProcessorBase
    {
        public AnglesharpWithXml(IChromeDriverWrapper driverWrapper, IHtmlProcessorFactory htmlProcessorFactory) : base(driverWrapper, htmlProcessorFactory)
        {
        }

        protected override IHtmlProcessor CreateHtmlProcessorInstance()
        {
            return HtmlProcessorFactory.GetAngleSharpHtmlProcessor(HtmlBody, true);
        }
    }
}
