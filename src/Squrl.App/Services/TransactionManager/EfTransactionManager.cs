using Microsoft.EntityFrameworkCore.Storage;
using Squrl.App.Data;

namespace Squrl.App.Services.TransactionManager;

public class EfTransactionManager : ITransactionManager
{
    private readonly DataContext _dataContext;

    public EfTransactionManager(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction = await _dataContext.Database.BeginTransactionAsync(cancellationToken);

        await action(cancellationToken);

        await _dataContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction = await _dataContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            T result = await action(cancellationToken);

            await _dataContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}