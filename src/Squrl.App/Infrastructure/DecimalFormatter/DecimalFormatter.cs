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
        throw new NotImplementedException();
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
}