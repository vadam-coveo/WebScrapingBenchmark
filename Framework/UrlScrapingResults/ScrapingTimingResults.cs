using CsvHelper.Configuration.Attributes;
using WebScrapingBenchmark.Framework.Reporting;
using WebScrapingBenchmark.Framework.ScenarioRunner;

namespace WebScrapingBenchmark.Framework.UrlScrapingResults;

public class ScrapingTimingResults : BaseUrlScrapingResult
{
    public TimeSpan GoToUrlTiming;
    public TimeSpan LoadTiming;
    public TimeSpan GetHtmlResultTiming;

    [Ignore]
    public List<ElementTiming> MetadataExtractionTiming = new();

    [Ignore]
    public List<ElementTiming> ContentExclusionTiming = new();

    [TypeConverter(typeof(LazyValueConverter<TimeSpan>))]
    public Lazy<TimeSpan> AverageMetadataExtraction => new Lazy<TimeSpan>(() => MetadataExtractionTiming.Select(timing => timing.Duration).Average());
    [TypeConverter(typeof(LazyValueConverter<TimeSpan>))]
    public Lazy<TimeSpan> AverageContentExclusion => new Lazy<TimeSpan>(() => ContentExclusionTiming.Select(timing => timing.Duration).Average());

    /*[TypeConverter(typeof(LazyValueConverter<TimeSpan>))]
    public Lazy<TimeSpan> FastestMetadataExtraction => new Lazy<TimeSpan>(() => MetadataExtractionTiming.Select(timing => timing.Duration).Min());
    [TypeConverter(typeof(LazyValueConverter<TimeSpan>))]
    public Lazy<TimeSpan> FastestContentExclusion => new Lazy<TimeSpan>(() => ContentExclusionTiming.Select(timing => timing.Duration).Min());

    [TypeConverter(typeof(LazyValueConverter<TimeSpan>))]
    public Lazy<TimeSpan> SlowestMetadataExtraction => new Lazy<TimeSpan>(() => MetadataExtractionTiming.Select(timing => timing.Duration).Max());
    [TypeConverter(typeof(LazyValueConverter<TimeSpan>))]
    public Lazy<TimeSpan> SlowestContentExclusion => new Lazy<TimeSpan>(() => ContentExclusionTiming.Select(timing => timing.Duration).Max());*/

    /*[TypeConverter(typeof(LazyValueConverter<TimeSpan>))]
    public Lazy<TimeSpan> MetadataExtractionSum => new Lazy<TimeSpan>(() => TimeSpan.FromMilliseconds(MetadataExtractionTiming.Sum(timing => timing.Duration.TotalMilliseconds)));

    [TypeConverter(typeof(LazyValueConverter<TimeSpan>))]
    public Lazy<TimeSpan> ContentExclusionSum => new Lazy<TimeSpan>(() => TimeSpan.FromMilliseconds(ContentExclusionTiming.Sum(timing => timing.Duration.TotalMilliseconds)));

    [TypeConverter(typeof(LazyValueConverter<TimeSpan>))]
    public Lazy<TimeSpan> TotalScrapingTime => new Lazy<TimeSpan>(() => LoadTiming + MetadataExtractionSum.Value + ContentExclusionSum.Value + GetHtmlResultTiming);*/

    public ScrapingTimingResults(string url, string scenarioName, string scraperName) : base(url, scenarioName, scraperName)
    {
    }
}