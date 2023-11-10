using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.Helpers;
using WebscrapingBenchmark.Core.Framework.Logging;

namespace WebscrapingBenchmark.Core.Framework.ChromeDriver
{
    public interface IChromeDriverWrapper
    {
        OpenQA.Selenium.Chrome.ChromeDriver Driver { get; }
        string GetHtml(string url, TimeSpan jsTimeout);
        string GetHtml(string url);
    }

    public class ChromeDriverWrapper : IDisposable, IChromeDriverWrapper
    {
        public static TimeSpan DefaultJsTimeoutDelay = TimeSpan.FromSeconds(2);
        
        private int _processId;
        public OpenQA.Selenium.Chrome.ChromeDriver Driver { get; }
        private ChromeDriverService Service { get; }
        private ICache<CachedRequest> Cache { get; }

        public ChromeDriverWrapper(ICache<CachedRequest> cache, string? userAgent = null)
        {
            Cache = cache;
            Service = ChromeDriverService.CreateDefaultService();
            Service.HideCommandPromptWindow = true;

            _processId = Service.ProcessId;

            var options = new ChromeOptions();
            options.AddArgument("--ignore-ssl-errors=true");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--blink-settings=imagesEnabled=false");

            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");

            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                ConsoleLogger.WriteLine($"Starting chromium on process {_processId} with user agent :", ConsoleColor.DarkGreen);
                ConsoleLogger.WriteLine(userAgent, ConsoleColor.White, 3);
                options.AddArgument("--user-agent=" + userAgent);
            }
            else
            {
                ConsoleLogger.WriteLine("Starting chromium with default user agent", ConsoleColor.DarkGreen);
            }

            options.PageLoadStrategy = PageLoadStrategy.Normal;
            options.AcceptInsecureCertificates = true;

            var location = FilesystemHelper.Solution.ChromiumPath;

            if (!File.Exists(location))
            {
                ConsoleLogger.WriteLine($"Can't find chrome.exe in location {location}", ConsoleColor.DarkRed);
            }

            options.BinaryLocation = location;

            Driver = new OpenQA.Selenium.Chrome.ChromeDriver(Service, options);

            if (_processId == 0)
            {
                _processId = Service.ProcessId;
                ConsoleLogger.WriteLine($"ProcessId = {_processId}", ConsoleColor.DarkGreen);
            }
        }

        public string GetHtml(string url)
        {
            return GetHtml(url, DefaultJsTimeoutDelay);
        }

        public string GetHtml(string url, TimeSpan jsTimeout)
        {
            var shouldSimulateRequestDelay = true;

            var result = Cache.GetOrAdd(url, _ =>
            {
                shouldSimulateRequestDelay = false;

                var start = DateTime.Now;
                Driver.Navigate().GoToUrl(url);
                Thread.Sleep(jsTimeout);

                return new CachedRequest
                {
                    Delay = DateTime.Now - start,
                    htmlBody = Driver.PageSource
                };
            });

            if (shouldSimulateRequestDelay)
            {
                Thread.Sleep(result.Delay);
            }

            return result.htmlBody;
        }

        public void Dispose()
        {
            ConsoleLogger.WriteLine($"Disposing of the chromedriver for process {_processId}", ConsoleColor.DarkYellow, 5);
            Driver.Close();
            Driver.Dispose();
            ConsoleLogger.WriteLine("waiting 1 second ......", ConsoleColor.DarkYellow, 5);
            Thread.Sleep(1000);

            ConsoleLogger.WriteLine($"Disposing of the ChromeDriverService for process {_processId}", ConsoleColor.DarkYellow, 5);
            Service.Dispose();
            ConsoleLogger.WriteLine("waiting 1 second ......", ConsoleColor.DarkYellow, 5);
            Thread.Sleep(1000);

            try
            {
                var process = Process.GetProcessById(_processId);
                ConsoleLogger.WriteLine($"Clearing process {_processId}", ConsoleColor.DarkYellow, 5);

                if (!process.HasExited)
                    process.Kill();
            }
            catch (Exception e) when (!e.Message.Contains("is not running"))
            {
                ConsoleLogger.WriteLine($"Something went wrong while killing process {_processId}", ConsoleColor.Red, 5);
                ConsoleLogger.WriteLine(e.Message, ConsoleColor.DarkRed);
                ConsoleLogger.WriteLine(e.StackTrace, ConsoleColor.DarkRed);
            }
            catch (Exception)
            {
                // do nothing
            }
        }
    }
}
