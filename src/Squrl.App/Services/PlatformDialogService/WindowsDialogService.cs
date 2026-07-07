using Squrl.App.Interop.Windows;

namespace Squrl.App.Services.PlatformDialogService;

public sealed class WindowsDialogService : IPlatformDialogService
{
    public void ShowInformation(string title, string message)
    {
        Show(title, message, User32.MessageBoxOptions.Ok | User32.MessageBoxOptions.IconInformation);
    }

    public void ShowWarning(string title, string message)
    {
        Show(title, message, User32.MessageBoxOptions.Ok | User32.MessageBoxOptions.IconWarning);
    }

    public void ShowError(string title, string message)
    {
        Show(title, message, User32.MessageBoxOptions.Ok | User32.MessageBoxOptions.IconError);
    }

    private static void Show(string title, string message, User32.MessageBoxOptions type)
    {
        User32.MessageBox(
            nint.Zero,
            message,
            title,
            type);
    }
}