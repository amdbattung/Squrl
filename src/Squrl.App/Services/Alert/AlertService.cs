using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Models;
using AlertObject = Squrl.App.Common.Alert;

namespace Squrl.App.Services.Alert;

public class AlertService : IAlertService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private Dictionary<Guid, AlertObject> _activeAlerts = new();
    public event Func<Task>? AlertsChanged;

    public AlertService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        IReadOnlyList<Item> items = await dataContext.Items
            .AsNoTracking()
            .Where(i => i.Quantity <= i.LowQuantityAlertThreshold)
            .OrderBy(i => EF.Property<Instant>(i, "DateCreated"))
            .ThenBy(i => i.Name)
            .ToListAsync(cancellationToken);

        _activeAlerts = items.ToDictionary(
            i => i.Id,
            i => new AlertObject
            {
                ItemId = i.Id,
                Message = $"Low stock warning for item {i.Name}. " +
                          $"Current stock of {i.Quantity:#,##0.##} " +
                          $"is on or below the threshold of {i.LowQuantityAlertThreshold:#,##0.##}."
            });
    }

    public async Task<Result<AlertObject>> SetAlertAsync(
        Guid itemId, string message = "",
        CancellationToken cancellationToken = default)
    {
        AlertObject alert = new AlertObject
        {
            ItemId = itemId,
            Message = message
        };

        _activeAlerts[itemId] = alert;

        if (AlertsChanged != null)
        {
            await AlertsChanged.Invoke();
            return Result<AlertObject>.Ok(alert);
        }

        return Result<AlertObject>.Fail()
            .WithFailureType(FailureType.NotFound);
    }

    public async Task<Result<AlertObject>> ClearAlertAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        AlertObject? alert = _activeAlerts.GetValueOrDefault(alertId);
        if (alert is not null)
        {
            _activeAlerts.Remove(alertId);
            
            if (AlertsChanged is not null)
            {
                await AlertsChanged.Invoke();
                return Result<AlertObject>.Ok(alert);
            }
        }
        
        return Result<AlertObject>.Fail()
            .WithFailureType(FailureType.NotFound);
    }

    public Task<Result<IReadOnlyCollection<AlertObject>>> GetAlertsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<AlertObject> alerts = _activeAlerts.Values.ToList().AsReadOnly();
        return Task.FromResult<Result<IReadOnlyCollection<AlertObject>>>(
            Result<IReadOnlyCollection<AlertObject>>.Ok(alerts));
    }
}