namespace Squrl.App.Services.PlatformDialogService;

public interface IPlatformDialogService
{
    void ShowInformation(string title, string message);
    void ShowWarning(string title, string message);
    void ShowError(string title, string message);
}