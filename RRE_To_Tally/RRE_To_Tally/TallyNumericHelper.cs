using System.Globalization;

namespace RRE_To_Tally;

public sealed class TallyNumericHelper
{
    private readonly IList<string> _warnings;

    public TallyNumericHelper(IList<string>? warnings = null)
    {
        _warnings = warnings ?? new List<string>();
    }

    public decimal ParseDecimal(object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return 0m;
        }

        if (value is decimal)
        {
            return (decimal)value;
        }

        string text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
        string cleaned = text.Replace(",", "")
            .Replace("₹", "")
            .Replace("â‚¹", "")
            .Replace("%", "")
            .Trim();

        if (cleaned.Length == 0)
        {
            return 0m;
        }

        decimal result;
        if (decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
        {
            return result;
        }

        _warnings.Add("Invalid numeric data: " + text);
        return 0m;
    }

    public decimal ParseGstRate(object? value)
    {
        decimal rate = ParseDecimal(value);
        if (rate < 0m)
        {
            _warnings.Add("Invalid GST rate: " + Convert.ToString(value, CultureInfo.InvariantCulture));
            return 0m;
        }

        return rate;
    }

    public static string FormatAmount(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero).ToString("0.00", CultureInfo.InvariantCulture);
    }

    public static string FormatQuantity(decimal value)
    {
        return value.ToString("0.###", CultureInfo.InvariantCulture);
    }

    public static string FormatGstRate(decimal value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
