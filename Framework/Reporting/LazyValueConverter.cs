using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace WebScrapingBenchmark.Framework.Reporting
{
    public class LazyValueConverter<T> : DefaultTypeConverter
    {
        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((Lazy<T>)value)?.Value?.ToString();
        }
    }
}
