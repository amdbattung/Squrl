namespace Squrl.App.Services.BackgroundTaskQueue;

public sealed  class BackgroundTaskService : BackgroundService
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly ILogger<BackgroundTaskService> _logger;

    public BackgroundTaskService(IBackgroundTaskQueue queue,
        ILogger<BackgroundTaskService> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Func<CancellationToken, ValueTask> workItem = await _queue.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }
    
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(BackgroundTaskService)} is stopping.");
        await base.StopAsync(stoppingToken);
    }
}