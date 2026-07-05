using Squrl.App.Common;

namespace Squrl.App.Services.Alert;

public interface IAlertService
{
    public event Func<Task>? AlertsChanged;
    public Task InitializeAsync(CancellationToken cancellationToken = default);
    public Task<Result<Common.Alert>> SetAlertAsync(Guid itemId, string message, CancellationToken cancellationToken = default);
    public Task<Result<Common.Alert>> ClearAlertAsync(Guid alertId, CancellationToken cancellationToken = default);
    public Task<Result<IReadOnlyCollection<Common.Alert>>> GetAlertsAsync(CancellationToken cancellationToken = default);
}