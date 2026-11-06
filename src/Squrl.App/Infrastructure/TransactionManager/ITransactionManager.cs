using Squrl.App.Common;

namespace Squrl.App.Infrastructure.TransactionManager;

public interface ITransactionManager
{
    Task<Result<T>> ExecuteAsync<T>(
        Func<CancellationToken, Task<Result<T>>> action,
        CancellationToken cancellationToken = default);
    
    Task<T?> ExecuteAsync<T>(
        Func<CancellationToken, Task<T?>> action,
        CancellationToken cancellationToken);
    
    void OnRollback(Func<Task> callback);
    
    void OnCommit(Func<Task> callback);
}