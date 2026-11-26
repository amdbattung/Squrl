using Microsoft.Win32;

namespace Squrl.App.Infrastructure.StartupRunner;

public class WindowsStartupRunner : IStartupRunner
{
    private const string RegistryPath =
        @"Software\Microsoft\Windows\CurrentVersion\Run";

    private const string AppName = "Squrl";

    private readonly string? _executablePath;

    public WindowsStartupRunner(IConfiguration configuration)
    {
        _executablePath = configuration["Startup:ExecutablePath"];
        
        if (string.IsNullOrWhiteSpace(_executablePath))
        {
            throw new InvalidOperationException("Missing startup executable path.");
        }

        if (!File.Exists(_executablePath))
        {
            throw new FileNotFoundException("The startup executable path does not exist.");
        }
    }
    
    public void SetEnabled(bool enabled)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException(
                "The current operating system does not support this functionality.");
        }
            
        using RegistryKey key =
            Registry.CurrentUser.CreateSubKey(RegistryPath);
        
        if (key is null)
        {
            throw new InvalidOperationException(
                "Unable to access the Windows startup registry key.");
        }

        if (enabled)
        {
            key.SetValue(
                AppName,
                $"\"{_executablePath}\"",
                RegistryValueKind.String);
        }
        else
        {
            key.DeleteValue(
                AppName,
                throwOnMissingValue: false);
        }
    }
}