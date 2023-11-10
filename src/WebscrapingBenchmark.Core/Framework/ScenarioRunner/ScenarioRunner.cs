using WebscrapingBenchmark.Core.Framework.Caching;
using WebscrapingBenchmark.Core.Framework.Config;
using WebscrapingBenchmark.Core.Framework.Interfaces;
using WebscrapingBenchmark.Core.Framework.Logging;
using WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

namespace WebscrapingBenchmark.Core.Framework.ScenarioRunner
{
    public class ScenarioRunner : IScenarioRunner
    {
        public IWebScraperStrategy WebScraper { get; }
        public ConfigurationScenario Scenario { get; }

        public ICache<CachedRequest> RequestCache { get; }
        
        private IAggregator<ScrapingMetrics> ScrapingAggregator { get; }
        public ScenarioRunner(IWebScraperStrategy webScraper, ConfigurationScenario scenario, IAggregator<ScrapingMetrics> scrapingAggregator, ICache<CachedRequest> requestCache)
        {
            WebScraper = webScraper;
            Scenario = scenario;
            ScrapingAggregator = scrapingAggregator;
            RequestCache = requestCache;
        }

        public void RunScenario()
        {
            ConsoleLogger.Debug("\r\r");
            ConsoleLogger.Info($"-------------------------------------Running scenario : {Scenario.ScenarioName} with scraper {WebScraper.GetType().Name}");

            foreach (var url in Scenario.Urls)
            {
                ScrapingAggregator.Aggregate(Evaluate(url));
            }

            ConsoleLogger.Info($"-------------------------------------------------------------------");
            ConsoleLogger.Debug("\r\r");
        }

        private ScrapingMetrics Evaluate(string url)
        {
           var results = new ScrapingMetrics(url, Scenario.ScenarioName, WebScraper.GetType().Name);

           ConsoleLogger.Debug("\r\r\r");

           EvaluateGoingToUrl(results);

           results.LoadTiming = Evaluate(() => WebScraper.Load());
           ConsoleLogger.Debug($"\t{FormatHelper.StringifyDuration(results.LoadTiming)} \t Loading libraries");

           ConsoleLogger.Debug("\r\r");

           EvaluateMetadataExtraction(results);
           EvaluateContentExclusions(results);

           ConsoleLogger.Debug("\r");
           EvaluateFinalHtmlBody(results);

           ConsoleLogger.Debug("\r\r");

           return results;
        }

        private void EvaluateGoingToUrl(ScrapingMetrics result)
        {
            var url = result.Url;
            string html = "";

            result.GoToUrlTiming = Evaluate(() => html = WebScraper.GoToUrl(url));

            if (WebScraper.UsesCachedRequests)
                result.GoToUrlTiming += RequestCache.TryGet(url)?.Delay ?? TimeSpan.Zero;

            result.RegisterInitialBody(html);

            ConsoleLogger.Info($"\t{FormatHelper.StringifyDuration(result.GoToUrlTiming)}  {FormatHelper.GetFormattedByes(result.InitialHtmlBytes, 16)} - Browsed to url {url}");
        }

        private void EvaluateContentExclusions(ScrapingMetrics result)
        {
            foreach (var selector in Scenario.Settings.Exclude)
            {
                try
                {
                    bool excludedContent = false;

                    var before = FormatHelper.GetBytes(WebScraper.GetCleanedHtml());

                    var duration = Evaluate(() => excludedContent = WebScraper.ExcludeHtml(selector!));

                    var after = FormatHelper.GetBytes(WebScraper.GetCleanedHtml());

                    var diff = before - after;

                    ConsoleLogger.Debug($"\t{FormatHelper.StringifyDuration(duration)} \t {FormatHelper.GetFormattedByes(diff, 10)} [{(excludedContent ? "+" : "-")}] excluded for {selector.Type} selector {selector.Path.TrimEnd('\r').TrimEnd('\n')}".TrimEnd());

                    result.RegisterContentExclusion(selector.Path, diff);
                    result.ContentExclusionTiming.Add(new ElementTiming
                    {
                        SelectorName = selector.Path,
                        Duration = duration
                    });
                }
                catch (Exception ex)
                {
                    ConsoleLogger.Error(
                        $"Unable to exclude html based on path {selector.Path} in scenario {Scenario.ScenarioName}", ex);
                }
            }
        }

        private void EvaluateMetadataExtraction(ScrapingMetrics result)
        {
            foreach (var metadata in Scenario.Settings.Metadata)
            {
                try
                {
                    IEnumerable<string> extraction = new List<string>();
                    var duration = Evaluate(() => { extraction = WebScraper.ExtractMetadata(metadata.Value); });

                    var bytes = extraction.Sum(FormatHelper.GetBytes);
                    ConsoleLogger.Debug($"\t{FormatHelper.StringifyDuration(duration)} \t {FormatHelper.GetFormattedByes(bytes, 10)} [{(bytes>0 ? "+" : "-")}] {extraction.Count()} values extracted for metadata '{metadata.Key}' with selector {metadata.Value.Path.TrimEnd('\r').TrimEnd('\n')}".TrimEnd());

                    result.RegisterMetadata(metadata.Value.Path, extraction);
                    result.MetadataExtractionTiming.Add(new ElementTiming
                    {
                        SelectorName = metadata.Value.Path,
                        Duration = duration
                    });
                }
                catch (Exception ex)
                {
                    ConsoleLogger.Error(
                        $"Unable to evaluate metadata extraction for key {metadata.Key} in scenario {Scenario.ScenarioName}", ex);
                }
            }
        }

        private void EvaluateFinalHtmlBody(ScrapingMetrics result)
        {
            var body = "";

            result.GetHtmlResultTiming = Evaluate(() =>
            {
                body = WebScraper.GetCleanedHtml();
            });

            result.RegisterFinalBody(body);

            ConsoleLogger.Info($"\t{FormatHelper.StringifyDuration(result.TotalScrapingTime.Value)} \t Scraping time {FormatHelper.GetFormattedByes(result.ExcludedBytes, 20)} removed from intial body");
        }

        private TimeSpan Evaluate(Action action)
        {
            var start = DateTime.Now;
            action.Invoke();
            return DateTime.Now - start;
        }
    }
}
