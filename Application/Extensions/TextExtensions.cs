using System.Globalization;
using Serilog;

namespace Affiliate.Application.Extensions;

public static class TextExtensions
{
    public static int CalculateReadingTime(this string Content)
    {
        // Average reading speed in words per minute
        int wordsPerMinute = 200;

        // Calculate the number of words in the content
        string[] words = Content.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        int wordCount = words.Length;

        // Calculate the estimated reading time in minutes
        int readingTime = (int)Math.Ceiling((double)wordCount / wordsPerMinute);

        return readingTime;
    }

    public static string GetRelativePath(this string path)
    {
        return Path.GetRelativePath(Path.Combine("wwwroot"), path)
            .Replace("\\", "/");
    }

    private static string? AddDecimalSeparator(this string? number)
    {
        try
        {
            var result = number;
            var phanThapPhan = "";
            if (!string.IsNullOrEmpty(number))
            {
                if (number.IndexOf(",", StringComparison.Ordinal) != -1)
                {
                    result = number.Split(",")[0];
                    phanThapPhan = number.Split(",")[1];
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                var negative = "";

                var afterComma = "";

                if (result.Contains("-"))
                {
                    negative = "-";
                    result = result.Replace("-", "");
                }

                if (result.Contains('.'))
                {
                    string?[] arr = result.Split('.');
                    result = arr[0];
                    afterComma = arr[1];
                }

                if (result is { Length: > 0 })
                {
                    var block = result.Length / 3;
                    var du = result.Length % 3;
                    if (du == 0) block -= 1;
                    if (block > 0)
                        for (var i = 1; i <= block; i++)
                        {
                            var pos = i;
                            if (pos == 1)
                                pos = pos * 3;
                            else
                                pos = pos * 3 + i - 1;
                            result = result.Insert(result.Length - pos, ".");
                        }
                }

                if (!string.IsNullOrEmpty(afterComma)) result += "," + afterComma;
                if (!string.IsNullOrEmpty(negative)) result = "-" + result;
                if (!string.IsNullOrEmpty(phanThapPhan))
                {
                    number += "," + phanThapPhan;
                }

                return result;
            }


            return number;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config page: {Exception}", exception);
            return number;
        }
    }

    public static string? AddDecimalSeparator(this long number)
    {
        return AddDecimalSeparator(number.ToString());
    }

    public static string? AddDecimalSeparator(this long? number)
    {
        if (number is > 0)
            return AddDecimalSeparator(number.ToString());
        return "0";
    }

    public static string? AddDecimalSeparator(this int number)
    {
        return AddDecimalSeparator(number.ToString());
    }

    public static string? AddDecimalSeparator(this int? number)
    {
        return number > 0 ? AddDecimalSeparator(number.ToString()) : "0";
    }

    public static string? AddDecimalSeparator(this double number)
    {
        return number > 0 ? AddDecimalSeparator(number.ToString(CultureInfo.InvariantCulture)) : "0";
    }

    public static string? AddDecimalSeparator(this double? number)
    {
        return number > 0 ? AddDecimalSeparator(number.ToString()) : "0";
    }

    public static string? AddDecimalSeparator(this decimal? number)
    {
        return number > 0 ? AddDecimalSeparator(number.ToString()) : "0";
    }

    public static string? AddDecimalSeparator(this decimal number)
    {
        return number > 0 ? AddDecimalSeparator(number.ToString(CultureInfo.InvariantCulture)) : "0";
    }
}