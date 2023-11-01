using OpenQA.Selenium.Chrome;
using WebScrapingBenchmark.Framework.ChromeDriver;
using WebScrapingBenchmark.Framework.Config;

namespace WebScrapingBenchmark.WebScrapingStrategies
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
        }

        public void ExcludeHtml(Selector selector)
        {
        }

        public string ExtractMetadata(Selector selector)
        {
            return null;
        }

        public string GetCleanedHtml()
        {
            return null;
        }
    }
}
