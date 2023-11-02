using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.Logging;
using WebScrapingBenchmark.Framework.UrlScrapingResults;
using WebScrapingBenchmark.WebScrapingStrategies;

namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class ScenarioRunner : IScenarioRunner
    {
        public IWebScraperStrategy WebScraper { get; }
        public ConfigurationScenario Scenario { get; }
        
        private IAggregator<ScrapingMetrics> ScrapingAggregator { get; }
        public ScenarioRunner(IWebScraperStrategy webScraper, ConfigurationScenario scenario, IAggregator<ScrapingMetrics> scrapingAggregator)
        {
            WebScraper = webScraper;
            Scenario = scenario;
            ScrapingAggregator = scrapingAggregator;
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

                    var duration = Evaluate(() => excludedContent = WebScraper.ExcludeHtml(selector));

                    var after = FormatHelper.GetBytes(WebScraper.GetCleanedHtml());

                    var diff = before - after;

                    ConsoleLogger.Debug($"\t{FormatHelper.StringifyDuration(duration)} \t {FormatHelper.GetFormattedByes(diff, 10)} [{(excludedContent ? "+" : "-")}] excluded for {selector.Type} selector {selector.Path}");

                    result.RegisterContentExclusion(selector.Path, excludedContent);
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
                    ConsoleLogger.Debug($"\t{FormatHelper.StringifyDuration(duration)} \t Extracted = {extraction.Count()} values for {metadata.Value.Type} selector {metadata.Value.Path}");

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
