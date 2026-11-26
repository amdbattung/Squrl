using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Infrastructure.DecimalFormatter;
using Squrl.App.Models;
using AlertObject = Squrl.App.Common.Alert;

namespace Squrl.App.Services.Alert;

public sealed class AlertService : IAlertService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDecimalFormatter _decimalFormatter;

    private Dictionary<Guid, AlertObject> _activeAlerts = new();
    private readonly object _alertsLock = new();
    
    public event Func<Task>? AlertsChanged;

    public AlertService(IServiceScopeFactory scopeFactory,
        IDecimalFormatter decimalFormatter)
    {
        _scopeFactory = scopeFactory;
        _decimalFormatter = decimalFormatter;
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

        Dictionary<Guid, AlertObject> alerts = items.ToDictionary(
            i => i.Id,
            i => new AlertObject
            {
                ItemId = i.Id,
                Message = $"Low stock warning for item {i.Name}. " +
                          $"Current stock of {_decimalFormatter.FormatQuantity(i.Quantity)} " +
                          $"is on or below the threshold of {_decimalFormatter
                              .RemoveTrailingZeros(i.LowQuantityAlertThreshold)}."
            });
        
        lock (_alertsLock)
        {
            _activeAlerts.Clear();

            foreach ((Guid id, AlertObject alert) in alerts)
            {
                _activeAlerts[id] = alert;
            }
        }

        await NotifyAlertsChangedAsync();
    }

    public async Task<Result<AlertObject>> SetAlertAsync(
        Guid itemId, string message = "",
        CancellationToken cancellationToken = default)
    {
        AlertObject alert = new()
        {
            ItemId = itemId,
            Message = message
        };

        lock (_alertsLock)
        {
            _activeAlerts[itemId] = alert;
        }

        await NotifyAlertsChangedAsync();

        return Result<AlertObject>.Ok(alert);
    }

    public async Task<Result<AlertObject>> ClearAlertAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        AlertObject? alert;

        lock (_alertsLock)
        {
            if (!_activeAlerts.Remove(alertId, out alert))
            {
                return Result<AlertObject>.Fail()
                    .WithFailureType(FailureType.NotFound);
            }
        }

        await NotifyAlertsChangedAsync();

        return Result<AlertObject>.Ok(alert);
    }

    public Task<Result<IReadOnlyCollection<AlertObject>>> GetAlertsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        IReadOnlyCollection<AlertObject> alerts;

        lock (_alertsLock)
        {
            alerts = _activeAlerts.Values
                .ToList()
                .AsReadOnly();
        }
        
        return Task.FromResult<Result<IReadOnlyCollection<AlertObject>>>(
            Result<IReadOnlyCollection<AlertObject>>.Ok(alerts));
    }
    
    private async Task NotifyAlertsChangedAsync()
    {
        Func<Task>? handlers = AlertsChanged;

        if (handlers is null)
            return;

        foreach (Delegate @delegate in handlers.GetInvocationList())
        {
            Func<Task> handler = (Func<Task>)@delegate;
            await handler();
        }
    }
}