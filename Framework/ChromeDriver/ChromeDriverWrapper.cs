using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebScrapingBenchmark.Framework.Logging;

namespace WebScrapingBenchmark.Framework.ChromeDriver
{
    public interface IChromeDriverWrapper
    {
        OpenQA.Selenium.Chrome.ChromeDriver Driver { get; }
        string GetHtml(string url, TimeSpan jsTimeout);
    }

    public class ChromeDriverWrapper : IDisposable, IChromeDriverWrapper
    {
        const string WINDOWS_CHROMIUM_RELATIVE_BINARY_PATH = @"Chromium\Application\chrome.exe";

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

            var location = GetChromiumPath();

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

        static string GetChromiumPath()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new
                    NotSupportedException(
                        $"The Web Driver using Chromium is currently not supported on the operating system: {RuntimeInformation.OSDescription}.");
            }

            var defaultChromiumEmbeddedPath =
                Path.Combine(Directory.GetCurrentDirectory().Replace("bin\\Debug\\net6.0", ""),
                    WINDOWS_CHROMIUM_RELATIVE_BINARY_PATH);

            var defaultWindowsChromiumPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    WINDOWS_CHROMIUM_RELATIVE_BINARY_PATH);

            if (File.Exists(defaultChromiumEmbeddedPath))
                return defaultChromiumEmbeddedPath;

            if (File.Exists(defaultWindowsChromiumPath))
                return defaultWindowsChromiumPath;

            throw new FileNotFoundException("The Chromium browser is not available.");
        }
    }
}
