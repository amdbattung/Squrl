using NodaTime;
using Squrl.App.Data;

namespace Squrl.App.Services.Report;

public sealed partial class ReportService : IReportService
{
    private readonly DataContext _dataContext;
    private readonly IClock _clock;

    private readonly DateTimeZone _timeZone;

    public ReportService(DataContext dataContext,
        IClock clock)
    {
        _dataContext = dataContext;
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