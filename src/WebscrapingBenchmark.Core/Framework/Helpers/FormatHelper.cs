using System.Globalization;
using System.Text;
using Humanizer;

namespace WebscrapingBenchmark.Core.Framework.Helpers
{
    public static class FormatHelper
    {
        public static Lazy<NumberFormatInfo> _NumberFormatForTicks = new(() => new NumberFormatInfo
        {
            NumberGroupSeparator = " ",
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 4
        });

        public static Lazy<NumberFormatInfo> _variableDecimalLengthNumberFormat = new(() => new NumberFormatInfo
        {
            NumberGroupSeparator = " ",
            NumberDecimalSeparator = ".",
        });


        public static Lazy<NumberFormatInfo> _excelNumberFormat = new(() => new NumberFormatInfo
        {
            NumberGroupSeparator = "",
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 4
        });

        public static string StringifyDuration(TimeSpan duration)
        {
            var milis = Convert.ToDecimal(duration.Ticks) / 10000;

            return (milis.ToString("n", _NumberFormatForTicks.Value) + " ms").PadLeft(17);
        }

        public static string StringifyDifference(TimeSpan duration, TimeSpan comparableDuration)
        {
            if (duration == comparableDuration && duration == TimeSpan.Zero)
            {
                return "N/A";
            }

            var difference = duration - comparableDuration;

            if (difference == TimeSpan.Zero) return FormatTwoColumns("", StringifyDuration(duration));

            var differenceSign = "+";

            if (difference < TimeSpan.Zero)
            {
                differenceSign = "-";
                difference = difference.Negate();
            }

            return FormatTwoColumns($"{FormatDuration(duration)} ms", $"{differenceSign}{FormatDuration(difference)} ms");
        }

        public static string FormatTwoColumns(string col1, string col2)
        {
            return col1.Trim().PadLeft(18) + col2.Trim().PadLeft(18);
        }

        public static TimeSpan Sum(this IEnumerable<TimeSpan> timespans)
        {
            var totalTicks = timespans.Sum(x => x.Ticks);
            return new TimeSpan(totalTicks);
        }

        public static string FormatDuration(TimeSpan duration)
        {
            var milis = Convert.ToDecimal(duration.Ticks) / 10000;
            return milis.ToString("n", _NumberFormatForTicks.Value);
        }

        public static string FormatDurationForExcel(TimeSpan duration)
        {
            var milis = Convert.ToDecimal(duration.Ticks) / 10000;
            return milis.ToString("n", _excelNumberFormat.Value);
        }

        public static string FormatNumber(double number, int precision = 2)
        {
            return Math.Round(number, precision).ToString("n", precision == 0 ? _variableDecimalLengthNumberFormat.Value : _NumberFormatForTicks.Value);
        }

        public static string FormatNumber(decimal number, int precision = 2)
        {
            return Math.Round(number, precision).ToString("n", _variableDecimalLengthNumberFormat.Value);
        }

        public static long GetBytes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            return Encoding.ASCII.GetByteCount(input);
        }

        public static string GetFormattedByes(long input, int padding = 0)
        {
            return input.Bytes().Humanize().PadLeft(padding);
        }

    }
}
