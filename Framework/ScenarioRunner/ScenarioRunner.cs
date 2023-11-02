using AngleSharp.Dom;
using Humanizer;
using Humanizer.Localisation;
using System;
using System.Globalization;
using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.Logging;
using WebScrapingBenchmark.Framework.ScrapingResultComparing;
using WebScrapingBenchmark.Framework.UrlScrapingResults;
using WebScrapingBenchmark.WebScrapingStrategies;

namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class ScenarioRunner : IScenarioRunner
    {
        public IWebScraperStrategy WebScraper { get; }
        public ConfigurationScenario Scenario { get; }
        
        private IAggregator<ScrapingOutput> ScrapingOutputAggregator { get; }
        private IAggregator<ScrapingTimingResults> ScrapingTimingAggregator { get; }
        public ScenarioRunner(IWebScraperStrategy webScraper, ConfigurationScenario scenario, IAggregator<ScrapingOutput> scrapingOutputAggregator, IAggregator<ScrapingTimingResults> scrapingTimingAggregator)
        {
            WebScraper = webScraper;
            Scenario = scenario;
            ScrapingOutputAggregator = scrapingOutputAggregator;
            ScrapingTimingAggregator = scrapingTimingAggregator;
        }

        public void RunScenario()
        {
            ConsoleLogger.Debug("\r\r");
            ConsoleLogger.Info($"-------------------------------------Running scenario : {Scenario.ScenarioName} with scraper {WebScraper.GetType().Name}");

            var benchmark = new Benchmark();
            benchmark.ScenarioName = Scenario.ScenarioName;
            benchmark.ScraperName = WebScraper.GetType().Name;

            foreach (var url in Scenario.Urls)
            {
                var scrapingResult = Evaluate(url);

                ScrapingOutputAggregator.Aggregate(scrapingResult.ScrapingOutput);
                ScrapingTimingAggregator.Aggregate(scrapingResult.Timing);
            }

            ConsoleLogger.Info($"-------------------------------------------------------------------");
            ConsoleLogger.Debug("\r\r");
        }

        private UrlScrapingResults Evaluate(string url)
        {
           var results = new UrlScrapingResults(WebScraper.GetType().Name, Scenario.ScenarioName, url);

           ConsoleLogger.Debug("\r\r\r");

           EvaluateGoingToUrl(results);

           results.Timing.LoadTiming = Evaluate(() => WebScraper.Load());
           ConsoleLogger.Debug($"\t{FormatHelper.StringifyDuration(results.Timing.LoadTiming)} Loading libraries");

           ConsoleLogger.Debug("\r\r");

           EvaluateMetadataExtraction(results);

           EvaluateContentExclusions(results);

           EvaluateFinalHtmlBody(results);

           return results;
        }

        private void EvaluateGoingToUrl(UrlScrapingResults result)
        {
            var url = result.ScrapingOutput.Url;
            string html = "";

            result.Timing.GoToUrlTiming = Evaluate(() => html = WebScraper.GoToUrl(url));
            result.ScrapingOutput.RegisterInitialBody(html);

            ConsoleLogger.Info($"\t{FormatHelper.StringifyDuration(result.Timing.GoToUrlTiming)}  {FormatHelper.GetFormattedByes(result.ScrapingOutput.InitialHtmlBodySize)} - Browsed to url {url}");
        }

        private void EvaluateContentExclusions(UrlScrapingResults result)
        {
            foreach (var selector in Scenario.Settings.Exclude)
            {
                try
                {
                    bool excludedContent = false;
                    var duration = Evaluate(() => excludedContent = WebScraper.ExcludeHtml(selector));
                    ConsoleLogger.Debug($"\t{FormatHelper.StringifyDuration(duration)} \t Excluded = {excludedContent} for {selector.Type} selector {selector.Path}");

                    result.ScrapingOutput.RegisterContentExclusion(selector.Path, excludedContent);
                    result.Timing.ContentExclusionTiming.Add(new ElementTiming
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

        private void EvaluateMetadataExtraction(UrlScrapingResults result)
        {
            foreach (var metadata in Scenario.Settings.Metadata)
            {
                try
                {
                    IEnumerable<string> extraction = new List<string>();
                    var duration = Evaluate(() => { extraction = WebScraper.ExtractMetadata(metadata.Value); });
                    ConsoleLogger.Debug($"\t{FormatHelper.StringifyDuration(duration)} \t Extracted = {extraction.Count()} values for {metadata.Value.Type} selector {metadata.Value.Path}");

                    result.ScrapingOutput.RegisterMetadata(metadata.Value.Path, extraction);
                    result.Timing.MetadataExtractionTiming.Add(new ElementTiming
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

        private void EvaluateFinalHtmlBody(UrlScrapingResults result)
        {
            var body = "";

            result.Timing.GetHtmlResultTiming = Evaluate(() =>
            {
                body = WebScraper.GetCleanedHtml();
            });

            result.ScrapingOutput.RegisterFinalBody(body);

            ConsoleLogger.Info($"\t{FormatHelper.StringifyDuration(result.Timing.TotalScrapingTime.Value)} \t Scraping time,  {FormatHelper.GetFormattedByes(result.ScrapingOutput.BodySizeDiff)} removed from intial body");
        }

        private TimeSpan Evaluate(Action action)
        {
            var start = DateTime.Now;
            action.Invoke();
            return DateTime.Now - start;
        }

        private class UrlScrapingResults
        {
            public ScrapingTimingResults Timing { get; }
            public ScrapingOutput ScrapingOutput { get; }

            public UrlScrapingResults(string scraperName, string scenarioName, string url)
            {
                ScrapingOutput = new ScrapingOutput(url, scenarioName, scraperName);
                Timing = new ScrapingTimingResults(url, scenarioName, scraperName);
            }
        }
    }
}
