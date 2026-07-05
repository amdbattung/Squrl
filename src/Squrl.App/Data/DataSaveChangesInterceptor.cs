using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NodaTime;

namespace Squrl.App.Data;

public class DataSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IClock _clock;

    public DataSaveChangesInterceptor(IClock clock)
    {
        _clock = clock;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        
        if (context == null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        foreach (var entry in context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        {
            if (entry.State == EntityState.Added &&
                entry.Metadata.FindProperty("DateCreated") != null)
            {
                entry.Property("DateCreated").CurrentValue = _clock.GetCurrentInstant();
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}