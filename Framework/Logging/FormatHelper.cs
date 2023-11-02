using System.Globalization;
using System.Text;
using Humanizer;
using static System.Text.ASCIIEncoding;

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

            return (milis.ToString("n", _numberFormat.Value) + " ms").PadLeft(30);
        }

        public static string FormatDuration(TimeSpan duration)
        {
            var milis = Convert.ToDecimal(duration.Ticks) / (decimal)10000;
            return milis.ToString("n", _numberFormat.Value);
        }

        public static long GetBytes(string input)
        {
            if(string.IsNullOrEmpty(input))
                return 0;

            return Encoding.ASCII.GetByteCount(input);
        }

        public static string GetFormattedByes(long input, int padding = 0)
        {
            return input.Bytes().Humanize().PadLeft(padding);
        }
    }
}
