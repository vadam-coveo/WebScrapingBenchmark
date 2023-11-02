using System.Globalization;

namespace WebScrapingBenchmark.Framework.Logging
{
    public static class FormatHelper
    {
        public static Lazy<NumberFormatInfo> _numberFormat = new(() => new NumberFormatInfo
        {
            NumberGroupSeparator = " ",
            NumberDecimalSeparator = ",",
            NumberDecimalDigits = 4
        });

        public static string StringifyDuration(TimeSpan duration)
        {
            var milis = Convert.ToDecimal(duration.Ticks) / (decimal)10000;

            return milis.ToString("n", _numberFormat.Value).PadLeft(15) + " ms \t\t".PadLeft(5);
        }
    }
}
