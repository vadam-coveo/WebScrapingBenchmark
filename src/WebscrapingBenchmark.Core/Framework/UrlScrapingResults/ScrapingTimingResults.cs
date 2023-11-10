using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using WebscrapingBenchmark.Core.Framework.Reporting;

namespace WebscrapingBenchmark.Core.Framework.UrlScrapingResults;

public class ScrapingTimingResults : BaseUrlScrapingResult
{
    [TypeConverter(typeof(TimeSpanConverter))]
    public TimeSpan GoToUrlTiming { get; set; }

    [TypeConverter(typeof(TimeSpanConverter))]
    public TimeSpan LoadTiming { get; set; }

    [TypeConverter(typeof(TimeSpanConverter))]
    public TimeSpan GetHtmlResultTiming { get; set; }

    [Ignore]
    public List<ElementTiming> MetadataExtractionTiming = new();

    [Ignore]
    public List<ElementTiming> ContentExclusionTiming = new();

    [JsonIgnore]
    [TypeConverter(typeof(LazyTimeSpanConverter))]
    public Lazy<TimeSpan> AverageMetadataExtraction => GetLazy(MetadataExtractionTiming, Eval.Average);

    [JsonIgnore]
    [TypeConverter(typeof(LazyTimeSpanConverter))]
    public Lazy<TimeSpan> AverageContentExclusion => GetLazy(ContentExclusionTiming, Eval.Average);

    [JsonIgnore]
    [TypeConverter(typeof(LazyTimeSpanConverter))]
    public Lazy<TimeSpan> FastestMetadataExtraction => GetLazy(MetadataExtractionTiming, Eval.Min);

    [JsonIgnore]
    [TypeConverter(typeof(LazyTimeSpanConverter))]
    public Lazy<TimeSpan> FastestContentExclusion => GetLazy(ContentExclusionTiming, Eval.Min);

    [JsonIgnore]
    [TypeConverter(typeof(LazyTimeSpanConverter))]
    public Lazy<TimeSpan> SlowestMetadataExtraction => GetLazy(MetadataExtractionTiming, Eval.Max);

    [JsonIgnore]
    [TypeConverter(typeof(LazyTimeSpanConverter))]
    public Lazy<TimeSpan> SlowestContentExclusion => GetLazy(ContentExclusionTiming, Eval.Max);

    [JsonIgnore]
    [TypeConverter(typeof(LazyTimeSpanConverter))]
    public Lazy<TimeSpan> TotalMetadataExtractionTime => GetLazy(MetadataExtractionTiming, Eval.Sum);

    [JsonIgnore]
    [TypeConverter(typeof(LazyTimeSpanConverter))]
    public Lazy<TimeSpan> TotalContentExclusionTime => GetLazy(ContentExclusionTiming, Eval.Sum);

    [JsonIgnore]
    [TypeConverter(typeof(LazyTimeSpanConverter))]
    public Lazy<TimeSpan> TotalScrapingTime => new (() => LoadTiming + TotalMetadataExtractionTime.Value + TotalContentExclusionTime.Value + GetHtmlResultTiming);

    // needed for serializer
    public ScrapingTimingResults()
    {
    }

    public ScrapingTimingResults(string url, string scenarioName, string scraperName) : base(url, scenarioName, scraperName)
    {
    }

    protected enum Eval
    {
        Min,
        Max,
        Average,
        Sum
    }
    protected Lazy<TimeSpan> GetLazy(IEnumerable<ElementTiming> list, Eval eval)
    {
        if (!list?.Any() ?? true)
        {
            return new Lazy<TimeSpan>(() => TimeSpan.Zero);
        }

        switch (eval)
        {
            case Eval.Min:
                return new Lazy<TimeSpan>(() => list.Select(timing => timing.Duration).Min());
                break;
            case Eval.Max:
                return new Lazy<TimeSpan>(() => list.Select(timing => timing.Duration).Max());
                break;
            case Eval.Average:
                return new Lazy<TimeSpan>(() => list.Select(timing => timing.Duration).Average());
                break;
            case Eval.Sum:
                return new Lazy<TimeSpan>(() => TimeSpan.FromTicks(list.Select(timing => timing.Duration).Sum(x=> x.Ticks)));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eval), eval, null);
        }
    }
}