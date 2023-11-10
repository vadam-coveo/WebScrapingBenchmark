using Coveo.Connectors.Utilities.Logging;
using Coveo.Connectors.Utilities.Web.Configuration;
using Coveo.Connectors.Utilities.Web.WebScraping;
using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.ChromeDriver;
using WebscrapingBenchmark.Core.Framework.Interfaces;

namespace WebscrapingBenchmark.BaselineExecutor.WebScrapingStrategies
{
    public class RealBaseline : IWebScraperStrategy, IDisposable
    {
        public bool UsesCachedRequests => true;

        private IDomExtractor DomExtractor;

        private ICache<CachedRequest> Cache;

        private string _htmlBody;

        public RealBaseline(ICache<CachedRequest> cache)
        {
            Cache = cache;
            FeatureFlagLoader.SetOverrideForCurrentThread(typeof(FeatureFlagParameters.AllowWebScrapingWithEmptyBody), true);
        }

        public string GoToUrl(string url)
        {
            var result = Cache.TryGet(url);
            if (result == null)
            {
                throw new Exception($"URL {url} is not loaded in cache!");
            }
            _htmlBody = result.htmlBody;
            return _htmlBody;
        }

        public void Load()
        {
            DomExtractor = new DomExtractor(_htmlBody, new VoidLogger());
        }

        public bool ExcludeHtml(Selector selector)
        {
            return DomExtractor.Remove(selector);
        }

        public IEnumerable<string?> ExtractMetadata(Selector selector)
        {
            var result = DomExtractor.Extract(selector);

            if(result == null)
                return Enumerable.Empty<string>();

            if (result is IEnumerable<object> objects)
                return objects.Select(x => x.ToString());

            else
                return new List<string> { result.ToString()!};
        }

        public string GetCleanedHtml()
        {
            return DomExtractor.Body;
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}
