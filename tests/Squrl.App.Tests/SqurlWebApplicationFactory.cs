using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Squrl.App.Data;
using Squrl.App.Infrastructure.BackgroundTaskQueue;

namespace Squrl.App.Tests;

public class SqurlWebApplicationFactory : WebApplicationFactory<Program>
{
    private string? _databasePath;
    
    public string DatabasePath => _databasePath
                                  ?? throw new InvalidOperationException(
                                      "The test database has not been initialized.");
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        string databaseDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "database");
        
        Directory.CreateDirectory(databaseDirectory);
        
        CleanupOldTestDatabases(databaseDirectory);
        
        _databasePath = Path.Combine(
            databaseDirectory,
            $"squrl-test-{Guid.NewGuid():N}.db");

        builder.UseSetting(
            "ConnectionStrings:SqurlContext",
            $"Data Source={_databasePath}");
        
        string imageDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "images");

        Directory.CreateDirectory(imageDirectory);
        
        CleanupOldImages(imageDirectory);

        builder.ConfigureServices((context, services) =>
        {
            context.Configuration["ImageDirectory"] = imageDirectory;
            
            foreach (ServiceDescriptor descriptor in services
                .Where(x =>
                    x.ServiceType == typeof(IHostedService) &&
                    x.ImplementationType == typeof(BackgroundTask))
                .ToList())
            {
                services.Remove(descriptor);
            }
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost host = base.CreateHost(builder);

        using IServiceScope scope = host.Services.CreateScope();

        DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();

        db.Database.Migrate();

        return host;
    }

    public async Task ResetDatabaseAsync()
    {
        using IServiceScope scope = Services.CreateScope();
        
        DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
    }

    public IServiceScope CreateScope()
    {
        return Services.CreateScope();
    }
    
    private static void CleanupOldTestDatabases(string databaseDirectory)
    {
        if (!Directory.Exists(databaseDirectory))
        {
            return;
        }

        DateTime cutoff = DateTime.UtcNow.AddDays(-7);

        foreach (string file in Directory.EnumerateFiles(
                     databaseDirectory,
                     "squrl-test-*.db"))
        {
            try
            {
                if (File.GetLastWriteTimeUtc(file) < cutoff)
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // Don't prevent the test suite from running because
                // an old database couldn't be deleted.
            }
        }
    }

    private static void CleanupOldImages(string imageDirectory)
    {
        if (!Directory.Exists(imageDirectory))
        {
            return;
        }

        DateTime cutoff = DateTime.UtcNow.AddHours(-8);
        
        HashSet<string> imageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif",
            ".bmp", ".webp", ".tif", ".tiff", ".ico"
        };

        foreach (string file in Directory.EnumerateFiles(imageDirectory))
        {
            try
            {
                if (imageExtensions.Contains(Path.GetExtension(file)) &&
                    File.GetLastWriteTimeUtc(file) < cutoff)
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // Don't prevent the test suite from running because
                // an old image couldn't be deleted.
            }
        }
    }
}