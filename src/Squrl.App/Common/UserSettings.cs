namespace Squrl.App.Common;

public sealed record UserSettings
{
    public string CurrencySymbol { get; init; } = "$";
    public int ItemQuantityPrecision { get; init; } = 2;
    public bool OpenOnStartup { get; init; } = false;
}