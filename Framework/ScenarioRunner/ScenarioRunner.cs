﻿using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.Logging;
using WebScrapingBenchmark.WebScrapingStrategies;

namespace WebScrapingBenchmark.Framework.ScenarioRunner
{
    public class ScenarioRunner : IScenarioRunner
    {
        public IWebScraperStrategy WebScraper { get; }
        public ConfigurationScenario Scenario { get; }
        public IBenchmarkAggregator BenchmarkAggregator { get; }

        public ScenarioRunner(IWebScraperStrategy webScraper, ConfigurationScenario scenario, IBenchmarkAggregator benchmarkAggregator)
        {
            WebScraper = webScraper;
            Scenario = scenario;
            BenchmarkAggregator = benchmarkAggregator;
        }

        public void RunScenario()
        {
            ConsoleLogger.Info($"Running scenario : {Scenario.ScenarioName} with scraper {WebScraper.GetType().Name} ");
            var benchmark = new Benchmark();
            benchmark.ScenarioName = Scenario.ScenarioName;
            benchmark.ScraperName = WebScraper.GetType().Name;

            foreach (var url in Scenario.Urls)
            {
                benchmark.BenchmarkPerUrl.Add(Evaluate(url));
            }

            BenchmarkAggregator.AddBenchmark(benchmark);

            ConsoleLogger.Warn($"-------------------------------Done with scenario : {Scenario.ScenarioName} with scraper {WebScraper.GetType().Name}------------------------------------");
            ConsoleLogger.Debug("\r\r");
        }

        private ScrapingBenchmarkResult Evaluate(string url)
        {
           var result = new ScrapingBenchmarkResult{Url = url};

           // todo : see if things need some catching 

           ConsoleLogger.Info($"    Evaluating url {url}");

           result.GoToUrlTiming = Evaluate(()=> WebScraper.GoToUrl(url));

           result.LoadTiming = Evaluate(() => WebScraper.Load());

           EvaluateMetadataExtraction(result);

           EvaluateContentExclusions(result);

           result.GetHtmlResultTiming = Evaluate(() => WebScraper.GetCleanedHtml());

           return result;
        }

        private void EvaluateContentExclusions(ScrapingBenchmarkResult result)
        {
            foreach (var selector in Scenario.Settings.Exclude)
            {
                try
                {
                    var timing = new ElementTiming
                    {
                        SelectorName = selector.Path,
                        Duration = Evaluate(()=> ExcludeHtml(selector))
                    };

                    result.ContentExclusionTiming.Add(timing);
                }
                catch (Exception ex)
                {
                    ConsoleLogger.Error(
                        $"Unable to exclude html based on path {selector.Path} in scenario {Scenario.ScenarioName}", ex);
                }
            }
        }

        private void EvaluateMetadataExtraction(ScrapingBenchmarkResult result)
        {
            foreach (var metadata in Scenario.Settings.Metadata)
            {
                try
                {
                    var timing = new ElementTiming
                    {
                        SelectorName = metadata.Value.Path,
                        Duration = Evaluate(() => ExtractMetadata(metadata.Value))
                    };

                    result.MetadataExtractionTiming.Add(timing);
                }
                catch (Exception ex)
                {
                    ConsoleLogger.Error(
                        $"Unable to evaluate metadata extraction for key {metadata.Key} in scenario {Scenario.ScenarioName}", ex);
                }
            }
        }

        private void ExcludeHtml(Selector selector)
        {
            var result = WebScraper.ExcludeHtml(selector);
            ConsoleLogger.Debug($"          Excluded = {result} for {selector.Type} selector {selector.Path}");
        }

        private void ExtractMetadata(Selector selector)
        {
            var result = WebScraper.ExtractMetadata(selector);
            ConsoleLogger.Debug($"          Extracted = {result.Count()} values for {selector.Type} selector {selector.Path}");
        }

        private TimeSpan Evaluate(Action action)
        {
            var start = DateTime.Now;
            action.Invoke();
            return DateTime.Now - start;
        }
    }
}
