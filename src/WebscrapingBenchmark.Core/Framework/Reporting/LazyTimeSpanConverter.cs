using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using WebscrapingBenchmark.Core.Framework.Helpers;

namespace WebscrapingBenchmark.Core.Framework.Reporting
{
    public class LazyTimeSpanConverter : DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            var casted = value as Lazy<TimeSpan>;
            return FormatHelper.FormatDurationForExcel(casted?.Value ?? TimeSpan.Zero);
        }
    }

    public class TimespanConverter : DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            var casted = (TimeSpan?)value ;
            return FormatHelper.FormatDurationForExcel(casted ?? TimeSpan.Zero);
        }
    }
}
