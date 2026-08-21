namespace Squrl.App.Services.TransactionManager;

public interface ITransactionManager
{
    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken);
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken);
}