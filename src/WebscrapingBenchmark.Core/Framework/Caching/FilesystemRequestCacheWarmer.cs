using WebscrapingBenchmark.Core.Framework.ChromeDriver;
using WebscrapingBenchmark.Core.Framework.Config;
using WebscrapingBenchmark.Core.Framework.Helpers;

namespace WebscrapingBenchmark.Core.Framework.Caching
{
    public class FilesystemRequestCacheWarmer : ICacheWarmer
    {
        private ICache<CachedRequest> Cache { get; }
        private IEnumerable<ConfigurationScenario> Scenarios { get; }
        private IChromeDriverWrapperFactory ChromeDriverFactory { get; }

        public FilesystemRequestCacheWarmer(IEnumerable<ConfigurationScenario> scenarios, ICache<CachedRequest> cache, IChromeDriverWrapperFactory chromeDriver)
        {
            Scenarios = scenarios;
            Cache = cache;
            ChromeDriverFactory = chromeDriver;
        }

        public void WarmCache()
        {
            IChromeDriverWrapper? chromeDriver = null;

            foreach (var url in Scenarios.SelectMany(x => x.Urls))
            {
                var filepath = GetRequestFilePath(url);
                CachedRequest request;

                if (File.Exists(filepath))
                {
                    request = FilesystemHelper.FromJsonFile<CachedRequest>(filepath);
                }
                else
                {
                    chromeDriver ??= ChromeDriverFactory.Create();
                    request = Generate(url, chromeDriver);
                    FilesystemHelper.ToJsonFile(request, filepath);
                }

                Cache.AddOrUpdate(url, request);
            }

            if (chromeDriver != null)
            {
                ChromeDriverFactory.Release(chromeDriver);
            }
        }

        private string GetRequestFilePath(string url)
        {
            return Path.Join(FilesystemHelper.Solution.RequestCacheDirectory, $"{FilesystemHelper.CreateMD5(url)}.json");
        }

        private CachedRequest Generate(string url, IChromeDriverWrapper chromeDriver)
        {
            var start = DateTime.Now;
            chromeDriver.Driver.Navigate().GoToUrl(url);
            Thread.Sleep(ChromeDriverWrapper.DefaultJsTimeoutDelay);

            return new CachedRequest
            {
                htmlBody = chromeDriver.Driver.PageSource,
                Delay = DateTime.Now - start
            };
        }
    }
}
