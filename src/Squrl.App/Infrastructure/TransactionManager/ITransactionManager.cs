namespace Squrl.App.Services.TransactionManager;

public interface ITransactionManager
{
    Task<T?> ExecuteAsync<T>(Func<CancellationToken, Task<T?>> action, CancellationToken cancellationToken);
}