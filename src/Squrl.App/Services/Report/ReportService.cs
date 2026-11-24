using NodaTime;
using Squrl.App.Data;
using Squrl.App.Infrastructure.DecimalFormatter;

namespace Squrl.App.Services.Report;

public sealed partial class ReportService : IReportService
{
    private readonly DataContext _dataContext;
    private readonly IDecimalFormatter _decimalFormatter;
    private readonly IClock _clock;

    private readonly DateTimeZone _timeZone;

    public ReportService(DataContext dataContext,
        IDecimalFormatter decimalFormatter,
        IClock clock)
    {
        _dataContext = dataContext;
        _decimalFormatter = decimalFormatter;
        _clock = clock;
        _timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
    }

    private sealed record ItemTransactionSummary(
        string Name,
        string Description,
        string Uom,
        decimal TotalQuantity,
        decimal TotalPrice);
}