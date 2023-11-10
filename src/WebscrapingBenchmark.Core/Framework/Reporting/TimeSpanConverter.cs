using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using WebscrapingBenchmark.Core.Framework.Logging;

namespace WebscrapingBenchmark.Core.Framework.Reporting
{
    public class TimeSpanConverter : DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null) return null;
            
            return FormatHelper.FormatDurationForExcel((TimeSpan)value);
        }
    }
}
