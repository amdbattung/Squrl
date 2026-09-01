using System.Diagnostics;

namespace Squrl.WindowsHost;

public sealed class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly Process _squrlProcess;
    private readonly string _url;
    private readonly CancellationTokenSource _shutdownCts = new();
    private readonly ToolStripMenuItem _openMenuItem;

    private bool _isReady;
    
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(2)
    };

    public TrayApplicationContext(
        Process squrlProcess,
        string url)
    {
        _squrlProcess = squrlProcess;
        _url = url;

        _notifyIcon = new NotifyIcon
        {
            Text = "Squrl - Starting...",
            Icon = SystemIcons.Application,
            Visible = true
        };

        ContextMenuStrip menu = new ContextMenuStrip();

        _openMenuItem = new ToolStripMenuItem(
            "Open Squrl")
        {
            Enabled = false
        };

        _openMenuItem.Click += (_, _) => OpenSqurl();

        menu.Items.Add(_openMenuItem);

        menu.Items.Add(
            "Exit",
            null,
            (_, _) => _ = ExitAsync());

        _notifyIcon.ContextMenuStrip = menu;

        _notifyIcon.DoubleClick += (_, _) => OpenSqurl();

        _ = WaitForSqurlAsync();
    }

    private async Task WaitForSqurlAsync()
    {
        while (!_shutdownCts.IsCancellationRequested)
        {
            if (_squrlProcess.HasExited)
            {
                _notifyIcon.Text = "Squrl - Stopped";

                MessageBox.Show(
                    "Squrl.exe stopped before the server became ready.",
                    "Squrl",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            try
            {
                using HttpResponseMessage response =
                    await HttpClient.GetAsync(
                        _url,
                        HttpCompletionOption.ResponseHeadersRead,
                        _shutdownCts.Token);

                if (response.IsSuccessStatusCode)
                {
                    _isReady = true;
                    _openMenuItem.Enabled = true;

                    _notifyIcon.Text = "Squrl - Running";

                    OpenSqurl();

                    return;
                }

                // Server responded, but with an error.
                _notifyIcon.Text =
                    $"Squrl - HTTP {(int)response.StatusCode}";

            }
            catch (HttpRequestException)
            {
                // Server isn't ready yet.
            }
            catch (TaskCanceledException)
            {
                if (_shutdownCts.IsCancellationRequested)
                {
                    return;
                }

                // Request timed out. Keep trying.
            }

            try
            {
                await Task.Delay(
                    TimeSpan.FromMilliseconds(500),
                    _shutdownCts.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }

    private void OpenSqurl()
    {
        if (!_isReady)
        {
            MessageBox.Show(
                "Squrl is still starting. Please try again in a moment.",
                "Squrl",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = _url,
            UseShellExecute = true
        });
    }

    private async Task ExitAsync()
    {
        await _shutdownCts.CancelAsync();

        try
        {
            _notifyIcon.Visible = false;

            if (!_squrlProcess.HasExited)
            {
                _squrlProcess.CloseMainWindow();

                if (!await WaitForExitAsync(_squrlProcess, TimeSpan.FromSeconds(5)))
                {
                    _squrlProcess.Kill(entireProcessTree: true);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Squrl Shutdown Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _squrlProcess.Dispose();
            _notifyIcon.Dispose();
            _shutdownCts.Dispose();

            Application.ExitThread();
        }
    }

    private static async Task<bool> WaitForExitAsync(Process process, TimeSpan timeout)
    {
        using CancellationTokenSource cts = new CancellationTokenSource(timeout);

        try
        {
            await process.WaitForExitAsync(cts.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }
}