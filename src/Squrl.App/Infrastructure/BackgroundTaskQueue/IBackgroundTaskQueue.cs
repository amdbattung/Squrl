namespace Squrl.App.Infrastructure.BackgroundTaskQueue;

public interface IBackgroundTaskQueue
{
    ValueTask QueueAsync(Func<CancellationToken, ValueTask> workItem);
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}