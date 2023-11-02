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

    [TypeConverter(typeof(LazyTimespanConverter))]
    public Lazy<TimeSpan> AverageMetadataExtraction => GetLazy(MetadataExtractionTiming, Eval.Average);

    [TypeConverter(typeof(LazyTimespanConverter))]
    public Lazy<TimeSpan> AverageContentExclusion => GetLazy(ContentExclusionTiming, Eval.Average);

    [TypeConverter(typeof(LazyTimespanConverter))]
    public Lazy<TimeSpan> FastestMetadataExtraction => GetLazy(MetadataExtractionTiming, Eval.Min);

    [TypeConverter(typeof(LazyTimespanConverter))]
    public Lazy<TimeSpan> FastestContentExclusion => GetLazy(ContentExclusionTiming, Eval.Min);

    [TypeConverter(typeof(LazyTimespanConverter))]
    public Lazy<TimeSpan> SlowestMetadataExtraction => GetLazy(MetadataExtractionTiming, Eval.Max);

    [TypeConverter(typeof(LazyTimespanConverter))]
    public Lazy<TimeSpan> SlowestContentExclusion => GetLazy(ContentExclusionTiming, Eval.Max);

    [TypeConverter(typeof(LazyTimespanConverter))]
    public Lazy<TimeSpan> TotalMetadataExtraction => GetLazy(MetadataExtractionTiming, Eval.Sum);

    [TypeConverter(typeof(LazyTimespanConverter))]
    public Lazy<TimeSpan> TotalContentExclusionTime => GetLazy(ContentExclusionTiming, Eval.Sum);

    [TypeConverter(typeof(LazyTimespanConverter))]
    public Lazy<TimeSpan> TotalScrapingTime => new (() => LoadTiming + TotalMetadataExtraction.Value + TotalContentExclusionTime.Value + GetHtmlResultTiming);

    public ScrapingTimingResults(string url, string scenarioName, string scraperName) : base(url, scenarioName, scraperName)
    {
    }

    private enum Eval
    {
        Min,
        Max,
        Average,
        Sum
    }
    private Lazy<TimeSpan> GetLazy(IEnumerable<ElementTiming> list, Eval eval)
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