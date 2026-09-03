namespace Squrl.App.Services.PlatformDialogService;

public sealed class NullPlatformDialogService : IPlatformDialogService
{
    public void ShowInformation(string title, string message) { }

    public void ShowWarning(string title, string message) { }

    public void ShowError(string title, string message) { }
}