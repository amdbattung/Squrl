namespace Squrl.App.UI.Shared;

public sealed class Debouncer :IDisposable
{
    private readonly Lock _lock = new();
    private CancellationTokenSource? _cts;
    private bool _disposed;

    public async Task DebounceAsync(
        Func<CancellationToken, Task> action,
        TimeSpan delay)
    {
        ArgumentNullException.ThrowIfNull(action);

        CancellationTokenSource cts;

        lock (_lock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            _cts?.Cancel();

            cts = new CancellationTokenSource();
            _cts = cts;
        }

        try
        {
            await Task.Delay(delay, cts.Token);

            lock (_lock)
            {
                if (_disposed ||
                    cts.IsCancellationRequested ||
                    !ReferenceEquals(_cts, cts))
                {
                    return;
                }
            }

            cts.Token.ThrowIfCancellationRequested();

            await action(cts.Token);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            // Expected when a newer call replaces this one.
        }
        finally
        {
            lock (_lock)
            {
                if (ReferenceEquals(_cts, cts))
                {
                    _cts = null;
                }
            }

            cts.Dispose();
        }
    }

    public void Cancel()
    {
        lock (_lock)
        {
            CancellationTokenSource? cts = _cts;
            _cts = null;

            cts?.Cancel();
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;

            CancellationTokenSource? cts = _cts;
            _cts = null;

            cts?.Cancel();
        }
    }
}