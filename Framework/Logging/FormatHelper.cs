﻿using System.Globalization;
using System.Text;
using Humanizer;

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

        public static Lazy<NumberFormatInfo> _excelNumberFormat = new(() => new NumberFormatInfo
        {
            NumberGroupSeparator = "",
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 4
        });

        public static string StringifyDuration(TimeSpan duration)
        {
            var milis = Convert.ToDecimal(duration.Ticks) / (decimal)10000;

            return (milis.ToString("n", _numberFormat.Value) + " ms").PadLeft(15);
        }

        public static string StringifyDurationDifference(TimeSpan duration, TimeSpan comparableDuration)
        {
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

        public static string FormatStrategyName(string name)
        {
            return name.PadRight(40);
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
