using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Squrl.WindowsHost;

ApplicationConfiguration.Initialize();

using Mutex mutex = new Mutex(
    initiallyOwned: true,
    name: @"Global\Squrl",
    createdNew: out bool createdNew);

if (!createdNew)
{
    MessageBox.Show(
        "Squrl is already running.",
        "Squrl",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);

    return;
}

try
{
    string environment =
        Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
        ?? Environments.Production;

    bool isDevelopment = string.Equals(
        environment,
        Environments.Development,
        StringComparison.OrdinalIgnoreCase);

    string squrlAppDirectory;
    IConfiguration configuration;

    // Find Squrl.App Configuration
    if (isDevelopment)
    {
        squrlAppDirectory = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                @"..\..\..\..\Squrl.App"));

        configuration = new ConfigurationBuilder()
            .SetBasePath(squrlAppDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .AddJsonFile(
                "appsettings.Development.json",
                optional: true,
                reloadOnChange: false)
            .Build();
    }
    else
    {
        squrlAppDirectory = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData),
            "Squrl");

        configuration = new ConfigurationBuilder()
            .SetBasePath(squrlAppDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .Build();
    }
    
    // Get Squrl URL from Configuration
    string? url = configuration["Kestrel:Endpoints:Http:Url"];
    
    url = string.IsNullOrWhiteSpace(url) ? configuration["Urls"] : url;

    if (string.IsNullOrWhiteSpace(url))
    {
        throw new InvalidOperationException( "Squrl URL is not configured.");
    }

    // Kestrel binds to 0.0.0.0,
    // but clients should connect through localhost.
    url = url.Replace(
        "0.0.0.0",
        "localhost",
        StringComparison.OrdinalIgnoreCase);
    
    // Start Squrl Server
    Process? process;

    if (isDevelopment)
    {
        // Run the Squrl.App project through dotnet.
        string squrlAppDll = Path.Combine(
            squrlAppDirectory,
            "bin",
            "Debug",
            "net10.0",
            "Squrl.App.dll");

        if (!File.Exists(squrlAppDll))
        {
            throw new FileNotFoundException(
                "Squrl.App.dll was not found. Build Squrl.App first.",
                squrlAppDll);
        }
        
        string dotnetExe = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "dotnet",
            "dotnet.exe");

        if (!File.Exists(dotnetExe))
        {
            dotnetExe = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".dotnet",
                "dotnet.exe");
        }

        if (!File.Exists(dotnetExe))
        {
            throw new FileNotFoundException(
                "dotnet.exe was not found.",
                dotnetExe);
        }

        process = Process.Start(new ProcessStartInfo
        {
            FileName = dotnetExe,
            Arguments = $"\"{squrlAppDll}\" --environment Development",
            WorkingDirectory = squrlAppDirectory,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        });
    }
    else
    {
        // Run the production squrl-server.exe.
        string squrlAppExe = Path.Combine(
            AppContext.BaseDirectory,
            "server",
            "squrl-server.exe");
        
        if (!File.Exists(squrlAppExe))
        {
            squrlAppExe = Path.Combine(
                AppContext.BaseDirectory,
                "server",
                "Squrl.App.exe");
        }

        if (!File.Exists(squrlAppExe))
        {
            throw new FileNotFoundException(
                "Squrl-server.exe was not found.",
                squrlAppExe);
        }

        process = Process.Start(new ProcessStartInfo
        {
            FileName = squrlAppExe,
            WorkingDirectory = Path.GetDirectoryName(squrlAppExe),

            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        });
    }

    if (process is null)
    {
        throw new InvalidOperationException("Unable to start Squrl Server.");
    }
    
    // Start tray application
    Application.Run(new TrayApplicationContext(
        process,
        url));
}
catch (Exception ex)
{
    MessageBox.Show(
        ex.ToString(),
        "Squrl Startup Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
}