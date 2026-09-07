namespace Squrl.App.Infrastructure.PlatformDialog;

public interface IPlatformDialog
{
    void ShowInformation(string title, string message);
    void ShowWarning(string title, string message);
    void ShowError(string title, string message);
}