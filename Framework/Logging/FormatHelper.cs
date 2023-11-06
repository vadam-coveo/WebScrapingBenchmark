using System.Globalization;
using System.Text;
using Humanizer;
using WebScrapingBenchmark.WebScrapingStrategies;

namespace WebScrapingBenchmark.Framework.Logging
{
    public static class FormatHelper
    {
        public static Lazy<NumberFormatInfo> _numberFormat = new(() => new NumberFormatInfo
        {
            NumberGroupSeparator = " ",
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 4
        });

        public static Lazy<NumberFormatInfo> _numberFormatNoDecimals = new(() => new NumberFormatInfo
        {
            NumberGroupSeparator = " ",
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 0
        });


        public static Lazy<NumberFormatInfo> _excelNumberFormat = new(() => new NumberFormatInfo
        {
            NumberGroupSeparator = "",
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 4
        });

        public static string StringifyDuration(TimeSpan duration)
        {
            var milis = Convert.ToDecimal(duration.Ticks) / (decimal)10000;

            return (milis.ToString("n", _numberFormat.Value) + " ms").PadLeft(17);
        }

        public static string StringifyDurationDifference(TimeSpan duration, TimeSpan comparableDuration)
        {
            if (duration == comparableDuration && duration == TimeSpan.Zero)
            {
                return "N/A";
            }

            var difference = duration - comparableDuration;

            if (difference == TimeSpan.Zero) return StringifyDuration(duration);

            var differenceSign = "+";

            if (difference < TimeSpan.Zero)
            {
                differenceSign = "-";
                difference = difference.Negate();
            }

            return $"{StringifyDuration(duration)} ({differenceSign}{StringifyDuration(difference)})";
        }

        public static string StringifyDifference(TimeSpan duration, TimeSpan comparableDuration)
        {
            var difference = duration - comparableDuration;

            if (difference == TimeSpan.Zero) return StringifyDuration(duration);

            var differenceSign = "+";

            if (difference < TimeSpan.Zero)
            {
                differenceSign = "-";
                difference = difference.Negate();
            }
            return $"{(FormatDuration(duration)+" ms").PadRight(20)} " + ($"({differenceSign}{FormatDuration(difference)} ms)".PadRight(20));
        }

        public static TimeSpan Sum(this IEnumerable<TimeSpan> timespans)
        {
            var totalTicks = timespans.Sum(x => x.Ticks);
            return new TimeSpan(totalTicks);
        }

        public static string FormatDuration(TimeSpan duration)
        {
            var milis = Convert.ToDecimal(duration.Ticks) / (decimal)10000;
            return milis.ToString("n", _numberFormat.Value);
        }

        public static string FormatDurationForExcel(TimeSpan duration)
        {
            var milis = Convert.ToDecimal(duration.Ticks) / (decimal)10000;
            return milis.ToString("n", _excelNumberFormat.Value);
        }

        public static string FormatNumber(double number, int precision = 2)
        { 
            return Math.Round(number, precision).ToString("n", precision == 0 ? _numberFormatNoDecimals.Value : _numberFormat.Value);
        }

        public static string FormatNumber(decimal number, int precision = 2)
        {
            return Math.Round(number, precision).ToString("n", _numberFormat.Value);
        }

        public static string FormatStrategyName(string name)
        {
            return name.PadRight(12);
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
