using WebScrapingBenchmark.Framework.Config;
using WebScrapingBenchmark.Framework.UrlScrapingResults;

namespace WebScrapingBenchmark.Framework.ScrapingResultComparing
{
    public class ScrapingOutput : BaseUrlScrapingResult, IEquatable<ScrapingOutput>
    {
        

        private Dictionary<string, IEnumerable<string>> Metadata = new();
        private Dictionary<string, bool> ContentExclusions = new();
        private string HtmlBody;

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
            HtmlBody = htmlBody;
        }

        public bool Equals(ScrapingOutput? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ScenarioName == other.ScenarioName && Url == other.Url && HtmlBody == other.HtmlBody && ContentExclusions.Equals(other.ContentExclusions) && Metadata.Equals(other.Metadata);
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
            return HashCode.Combine(ScenarioName, Url, HtmlBody, ContentExclusions, Metadata);
        }
    }
}
