namespace Squrl.App.Infrastructure.PlatformDialogService;

public sealed class NullPlatformDialog : IPlatformDialog
{
    public void ShowInformation(string title, string message) { }

    public void ShowWarning(string title, string message) { }

    public void ShowError(string title, string message) { }
}