using Microsoft.EntityFrameworkCore.Storage;
using Squrl.App.Common;
using Squrl.App.Data;

namespace Squrl.App.Infrastructure.TransactionManager;

public sealed class EfTransactionManager : ITransactionManager
{
    private readonly DataContext _dataContext;
    
    private bool _rollbackOnly;

    public EfTransactionManager(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Result<T>> ExecuteAsync<T>(
        Func<CancellationToken, Task<Result<T>>> action,
        CancellationToken cancellationToken)
    {
        if (_dataContext.Database.CurrentTransaction is not null)
        {
            try
            {
                Result<T> result = await action(cancellationToken);
                
                if (!result.IsSuccess)
                {
                    _rollbackOnly = true;
                }
                
                return result;
            }
            catch
            {
                _rollbackOnly = true;
                throw;
            }
        }
        
        _rollbackOnly = false;
        
        await using IDbContextTransaction transaction = await _dataContext.Database
            .BeginTransactionAsync(cancellationToken);
        
        try
        {
            Result<T> result = await action(cancellationToken);
            
            if (_rollbackOnly || !result.IsSuccess)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                return result;
            }
            
            await _dataContext.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        finally
        {
            _rollbackOnly = false;
        }
    }

    public async Task<T?> ExecuteAsync<T>(
        Func<CancellationToken, Task<T?>> action,
        CancellationToken cancellationToken)
    {
        if (_dataContext.Database.CurrentTransaction is not null)
        {
            try
            {
                T? result = await action(cancellationToken);

                if (result is null)
                    _rollbackOnly = true;

                return result;
            }
            catch
            {
                _rollbackOnly = true;
                throw;
            }
        }

        _rollbackOnly = false;

        await using IDbContextTransaction transaction = await _dataContext.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            T? result = await action(cancellationToken);

            if (_rollbackOnly || result is null)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                return default;
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
        finally
        {
            _rollbackOnly = false;
        }
    }
}