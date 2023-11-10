using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.ChromeDriver;
using WebscrapingBenchmark.Core.Framework.Interfaces;
using WebscrapingBenchmark.NewStrategiesExecutor.HtmlProcessors;

namespace WebscrapingBenchmark.NewStrategiesExecutor.WebScrapingStrategies
{
    public class AgilityPkV2 : HtmlProcessorBase
    {
        public AgilityPkV2(IHtmlProcessorFactory htmlProcessorFactory, ICache<CachedRequest> requestCache) : base(htmlProcessorFactory, requestCache)
        {
        }

        protected override IHtmlProcessor CreateHtmlProcessorInstance()
        {
            return HtmlProcessorFactory.GetRevampedHtmlAgilityHtmlProcessor(HtmlBody);
        }
    }
}
