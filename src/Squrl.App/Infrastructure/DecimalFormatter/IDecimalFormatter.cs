namespace Squrl.App.Infrastructure.DecimalFormatter;

public interface IDecimalFormatter
{
    string FormatQuantity(decimal value);
    string FormatQuantity(decimal? value);
    string FormatCurrency(decimal value);
    string FormatCurrency(decimal? value);
    string RemoveTrailingZeros(decimal value);
    string RemoveTrailingZeros(decimal? value);
}