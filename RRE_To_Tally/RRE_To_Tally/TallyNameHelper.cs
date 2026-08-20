using System.Text;
using System.Text.RegularExpressions;

namespace RRE_To_Tally;

public static class TallyNameHelper
{
    public static string NormalizeUom(string? value)
    {
        string uom = CleanTallyName(value);
        string key = uom.Replace(".", "").Trim().ToUpperInvariant();
        if (key == "NO" || key == "NOS" || key == "NUMBER" || key == "NUMBERS") return "NOS";
        if (key == "PC" || key == "PCS" || key == "PIECE" || key == "PIECES") return "PCS";
        if (key == "MTR" || key == "METRE" || key == "METER") return "MTR";
        if (key == "KG" || key == "KGS" || key == "KILOGRAM") return "KGS";
        if (key == "LTR" || key == "LITRE" || key == "LITER") return "LTR";
        return string.IsNullOrWhiteSpace(uom) ? "NOS" : uom.ToUpperInvariant();
    }

    public static string CleanTallyName(string? value)
    {
        string text = CleanXmlText(value);
        text = Regex.Replace(text, @"\s+", " ").Trim();
        if (text.Length > 120)
        {
            text = text.Substring(0, 120).Trim();
        }

        return text;
    }

    public static string CleanXmlText(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        StringBuilder builder = new StringBuilder(value.Length);
        foreach (char ch in value)
        {
            if (ch == 0x9 || ch == 0xA || ch == 0xD || ch >= 0x20)
            {
                builder.Append(ch);
            }
        }

        return builder.ToString();
    }

    public static string GetTallyCustomerName(SalesExportRow row)
    {
        string name = CleanTallyName(row.MasterCustomerName);
        if (string.IsNullOrWhiteSpace(name))
        {
            name = CleanTallyName(row.SalesCustomerName);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            name = "CASH CUSTOMER";
        }

        return name;
    }

    public static string GetTallyProductName(SalesExportRow row)
    {
        string name = CleanTallyName(row.ProductName);
        if (string.IsNullOrWhiteSpace(name))
        {
            name = CleanTallyName(row.ItemCode);
        }

        return name;
    }

    public static bool IsBasicValidGstin(string? value)
    {
        string gstin = new string(((value ?? "").Trim()).Where(ch => !char.IsWhiteSpace(ch)).ToArray()).ToUpperInvariant();
        return Regex.IsMatch(gstin, @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][1-9A-Z]Z[0-9A-Z]$");
    }

    public static int GetUnitDecimalPlaces(string unit)
    {
        string normalized = NormalizeUom(unit);
        if (normalized == "KGS" || normalized == "MTR" || normalized == "LTR") return 3;
        return 0;
    }

    public static string SalesLedgerName(decimal gstRate)
    {
        return GetSalesLedgerName(gstRate, "SALES");
    }

    public static string GetSalesLedgerName(decimal gstRate, string? prefix)
    {
        string cleanPrefix = CleanTallyName(prefix);
        if (string.IsNullOrWhiteSpace(cleanPrefix)) cleanPrefix = "Sales";
        return cleanPrefix;
    }
}
