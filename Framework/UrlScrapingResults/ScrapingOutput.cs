﻿using WebScrapingBenchmark.Framework.Logging;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults
{
    public class ScrapingOutput : ScrapingTimingResults, IEquatable<ScrapingOutput>
    {
        public readonly Dictionary<string, IEnumerable<string>> Metadata = new();
        public readonly Dictionary<string, bool> ContentExclusions = new();
        public string FinalHtmlBody;
        public long InitialHtmlBytes { get; private set; }
        public long FinalHtmlBytes { get; private set; }
        public long ExcludedBytes { get; private set; }
        
        public ScrapingOutput(string url, string scenarioName, string scraperName) :base(url, scenarioName, scraperName)
        {
        }

        public void RegisterMetadata(string selectorPath, IEnumerable<string> result)
        {
            Metadata.Add(selectorPath, result);
        }

        public void RegisterContentExclusion(string selectorPath, bool excludedContent)
        {
            ContentExclusions.Add(selectorPath, excludedContent);
        }

        public void RegisterFinalBody(string htmlBody)
        {
            FinalHtmlBody = htmlBody;
            FinalHtmlBytes = FormatHelper.GetBytes(htmlBody);

            ExcludedBytes = InitialHtmlBytes - FinalHtmlBytes;
        }

        public void RegisterInitialBody(string htmlBody)
        {
            InitialHtmlBytes = FormatHelper.GetBytes(htmlBody);
        }

        public bool Equals(ScrapingOutput? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ScenarioName == other.ScenarioName && Url == other.Url && FinalHtmlBody == other.FinalHtmlBody && ContentExclusions.Equals(other.ContentExclusions) && Metadata.Equals(other.Metadata);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScrapingOutput)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ScenarioName, Url, FinalHtmlBody, ContentExclusions, Metadata);
        }
    }
}
