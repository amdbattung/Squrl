using Microsoft.EntityFrameworkCore.Storage;
using Squrl.App.Common;
using Squrl.App.Data;

namespace Squrl.App.Infrastructure.TransactionManager;

public sealed class EfTransactionManager : ITransactionManager
{
    private readonly DataContext _dataContext;
    
    private bool _rollbackOnly;
    
    private readonly List<Func<Task>> _rollbackCallbacks = new();
    
    private readonly List<Func<Task>> _commitCallbacks = new();

    public EfTransactionManager(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Result<T>> ExecuteAsync<T>(
        Func<CancellationToken, Task<Result<T>>> action,
        CancellationToken cancellationToken)
    {
        // Reuse an existing transaction for nested calls.
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
        _rollbackCallbacks.Clear();
        _commitCallbacks.Clear();
        
        await using IDbContextTransaction transaction = await _dataContext.Database
            .BeginTransactionAsync(cancellationToken);
        
        bool rollbackHandled = false;
        
        try
        {
            Result<T> result = await action(cancellationToken);
            
            if (_rollbackOnly || !result.IsSuccess)
            {
                await RollbackAndCleanupAsync(transaction);
                rollbackHandled = true;
                
                if (_rollbackOnly && result.IsSuccess)
                {
                    throw new InvalidOperationException("The transaction was rolled back because a nested operation failed.");
                }
                
                return result;
            }
            
            await _dataContext.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            await ExecuteCommitCallbacksAsync();
            
            return result;
        }
        catch
        {
            if (!rollbackHandled)
            {
                await RollbackAndCleanupAsync(transaction);
            }
            
            throw;
        }
        finally
        {
            _rollbackOnly = false;
            _rollbackCallbacks.Clear();
            _commitCallbacks.Clear();
        }
    }

    public async Task<T?> ExecuteAsync<T>(
        Func<CancellationToken, Task<T?>> action,
        CancellationToken cancellationToken)
    {
        // Reuse an existing transaction for nested calls.
        if (_dataContext.Database.CurrentTransaction is not null)
        {
            try
            {
                T? result = await action(cancellationToken);

                if (result is null)
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
        _rollbackCallbacks.Clear();
        _commitCallbacks.Clear();

        await using IDbContextTransaction transaction = await _dataContext.Database
            .BeginTransactionAsync(cancellationToken);
        
        bool rollbackHandled = false;

        try
        {
            T? result = await action(cancellationToken);

            if (_rollbackOnly || result is null)
            {
                await RollbackAndCleanupAsync(transaction);
                rollbackHandled = true;
                
                return default;
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            
            await ExecuteCommitCallbacksAsync();

            return result;
        }
        catch
        {
            if (!rollbackHandled)
            {
                await RollbackAndCleanupAsync(transaction);
            }
            
            throw;
        }
        finally
        {
            _rollbackOnly = false;
            _rollbackCallbacks.Clear();
            _commitCallbacks.Clear();
        }
    }
    
    public void OnRollback(Func<Task> callback)
    {
        if (_dataContext.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException("No active transaction.");
        }

        _rollbackCallbacks.Add(callback);
    }
    
    public void OnCommit(Func<Task> callback)
    {
        if (_dataContext.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException("Cannot register a commit callback without an active transaction.");
        }

        _commitCallbacks.Add(callback);
    }
    
    private async Task RollbackAndCleanupAsync(IDbContextTransaction transaction)
    {
        try
        {
            await transaction.RollbackAsync(CancellationToken.None);
        }
        catch
        {
            // TODO: Logs
            // Ignore.
        }
        finally
        {
            // Execute in reverse registration order.
            foreach (Func<Task> callback in _rollbackCallbacks.AsEnumerable().Reverse())
            {
                try
                {
                    await callback();
                }
                catch
                {
                    // TODO: Logs
                    // Continue executing the remaining callbacks.
                }
            }

            _rollbackCallbacks.Clear();
        }
    }
    
    private async Task ExecuteCommitCallbacksAsync()
    {
        foreach (Func<Task> callback in _commitCallbacks)
        {
            try
            {
                await callback();
            }
            catch (Exception)
            {
                // TODO: Logs
                // The database has already committed.
            }
        }

        _commitCallbacks.Clear();
    }
}