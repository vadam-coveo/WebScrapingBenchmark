using WebScrapingBenchmark.Framework.ScenarioRunner;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults;

public class ScrapingTimingResults : BaseUrlScrapingResult
{
    public TimeSpan GoToUrlTiming;
    public TimeSpan LoadTiming;
    public TimeSpan GetHtmlResultTiming;


    public List<ElementTiming> MetadataExtractionTiming = new();

    public List<ElementTiming> ContentExclusionTiming = new();

    public Lazy<TimeSpan> AverageMetadataExtraction => new Lazy<TimeSpan>(() => MetadataExtractionTiming.Select(timing => timing.Duration).Average());
    public Lazy<TimeSpan> AverageContentExclusion => new Lazy<TimeSpan>(() => ContentExclusionTiming.Select(timing => timing.Duration).Average());

    public Lazy<TimeSpan> FastestMetadataExtraction => new Lazy<TimeSpan>(() => MetadataExtractionTiming.Select(timing => timing.Duration).Min());
    public Lazy<TimeSpan> FastestContentExclusion => new Lazy<TimeSpan>(() => ContentExclusionTiming.Select(timing => timing.Duration).Min());

    public Lazy<TimeSpan> SlowestMetadataExtraction => new Lazy<TimeSpan>(() => MetadataExtractionTiming.Select(timing => timing.Duration).Max());
    public Lazy<TimeSpan> SlowestContentExclusion => new Lazy<TimeSpan>(() => ContentExclusionTiming.Select(timing => timing.Duration).Max());

    public Lazy<TimeSpan> MetadataExtractionSum => new Lazy<TimeSpan>(() => TimeSpan.FromMilliseconds(MetadataExtractionTiming.Sum(timing => timing.Duration.TotalMilliseconds)));

    public Lazy<TimeSpan> ContentExclusionSum => new Lazy<TimeSpan>(() => TimeSpan.FromMilliseconds(ContentExclusionTiming.Sum(timing => timing.Duration.TotalMilliseconds)));

    public Lazy<TimeSpan> TotalScrapingTime => new Lazy<TimeSpan>(() => LoadTiming + MetadataExtractionSum.Value + ContentExclusionSum.Value + GetHtmlResultTiming);

    public ScrapingTimingResults(string url, string scenarioName, string scraperName) : base(url, scenarioName, scraperName)
    {
    }
}