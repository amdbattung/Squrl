namespace Squrl.App.UI.Shared;

public class ToStringFormatters
{
    public static string FormatDecimal(decimal value)
    {
        if (value == decimal.Truncate(value))
        {
            return value.ToString("#,##0");
        }

        string s = value.ToString("G29"); // removes trailing zeros

        int dot = s.IndexOf('.');
        int decimals = dot >= 0 ? s.Length - dot - 1 : 0;

        if (decimals <= 2)
            return value.ToString("#,##0.00");

        return value.ToString("#,##0.############################");
    }
}