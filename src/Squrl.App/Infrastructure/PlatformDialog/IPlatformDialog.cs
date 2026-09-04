namespace Squrl.App.Infrastructure.PlatformDialogService;

public interface IPlatformDialog
{
    void ShowInformation(string title, string message);
    void ShowWarning(string title, string message);
    void ShowError(string title, string message);
}