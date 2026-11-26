using Squrl.App.Services.Settings;

namespace Squrl.App.Infrastructure.DecimalFormatter;

public class DecimalFormatter : IDecimalFormatter
{
    private readonly ISettingsService _settingsService;

    public DecimalFormatter(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }
    
    public string FormatQuantity(decimal value)
    {
        int precision = Math.Clamp(
            _settingsService.Current.ItemQuantityPrecision,
            0,
            6);

        value = Math.Round(
            value,
            precision,
            MidpointRounding.AwayFromZero);

        string format = precision > 0
            ? "#,##0." + new string('#', precision)
            : "#,##0";

        return value.ToString(format);
    }
    
    public string FormatQuantity(decimal? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        int precision = Math.Clamp(
            _settingsService.Current.ItemQuantityPrecision,
            0,
            6);

        decimal roundedValue = Math.Round(
            value.Value,
            precision,
            MidpointRounding.AwayFromZero);

        string format = precision > 0
            ? "#,##0." + new string('#', precision)
            : "#,##0";

        return roundedValue.ToString(format);
    }

    public string FormatCurrency(decimal value)
    {
        return $"{_settingsService.Current.CurrencySymbol}{value:N2}";
    }
    
    public string FormatCurrency(decimal? value)
    {
        return value == null
            ? string.Empty
            : $"{_settingsService.Current.CurrencySymbol}{value:N2}";
    }

    public string RemoveTrailingZeros(decimal value)
    {
        decimal rounded = Math.Round(value, 6, MidpointRounding.AwayFromZero);

        return rounded.ToString("#,##0.############################");
    }

    public string RemoveTrailingZeros(decimal? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        decimal rounded = Math.Round(value.Value, 6, MidpointRounding.AwayFromZero);

        return rounded.ToString("#,##0.############################");
    }
}