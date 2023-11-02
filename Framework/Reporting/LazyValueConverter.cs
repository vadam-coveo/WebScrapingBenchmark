using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using WebScrapingBenchmark.Framework.Logging;

namespace WebScrapingBenchmark.Framework.Reporting
{
    public class LazyValueConverter<T> : DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((Lazy<T>)value)?.Value?.ToString();
        }
    }

    public class LazyTimespanConverter : DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            var casted = value as Lazy<TimeSpan>;
            return FormatHelper.FormatDurationForExcel(casted?.Value ?? TimeSpan.Zero);
        }
    }
}
