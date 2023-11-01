using OpenQA.Selenium.Chrome;
using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.WebScrapingStrategies.Baseline
{
    public class BaselineStrategy : IWebScraperStrategy
    {
        private IChromeDriverWrapper DriverWrapper { get; }
        private ChromeDriver Driver => DriverWrapper.Driver;

        private string htmlbody;

        public BaselineStrategy(IChromeDriverWrapper driverWrapper)
        {
            DriverWrapper = driverWrapper;
        }

        public void GoToUrl(string url)
        {
            htmlbody = DriverWrapper.GetHtml(url, TimeSpan.FromSeconds(2));
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void ExcludeHtml(Selector selector)
        {
            throw new NotImplementedException();
        }

        public string ExtractMetadata(Selector selector)
        {
            throw new NotImplementedException();
        }

        public string GetCleanedHtml()
        {
            throw new NotImplementedException();
        }
    }
}
