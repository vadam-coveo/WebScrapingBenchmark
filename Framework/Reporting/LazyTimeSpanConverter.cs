using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using WebScrapingBenchmark.Framework.Logging;

namespace WebScrapingBenchmark.Framework.Reporting
{
    public class LazyTimeSpanConverter : DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            var casted = value as Lazy<TimeSpan>;
            return FormatHelper.FormatDurationForExcel(casted?.Value ?? TimeSpan.Zero);
        }
    }
}
