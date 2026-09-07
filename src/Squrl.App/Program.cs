using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Serilog;
using Serilog.Events;
using Squrl.App.Data;
using Squrl.App.Extensions;
using Squrl.App.Infrastructure.BackgroundTaskQueue;
using Squrl.App.Infrastructure.PlatformDialog;
using Squrl.App.Infrastructure.TransactionManager;
using Squrl.App.Services.Alert;
using Squrl.App.Services.Inventory;
using Squrl.App.Services.PurchaseOrder;
using Squrl.App.Services.Supplier;
using Squrl.App.Services.UnitOfMeasure;
using Squrl.App.UI;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up Squrl");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    
    if (OperatingSystem.IsWindows() && builder.Environment.IsProduction())
    {
        {
            string commonData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Squrl");

            builder.Configuration.AddJsonFile(
                Path.Combine(commonData, "appsettings.json"),
                optional: false,
                reloadOnChange: true);
        }
    }

    // Add services to the container.
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));
    
    builder.Services.AddHostedService<BackgroundTask>();
    builder.Services.AddSingleton<IBackgroundTaskQueue>(_ => 
    {
        if (!int.TryParse(builder.Configuration["QueueCapacity"], out int queueCapacity))
        {
            queueCapacity = 100;
        }

        return new BackgroundTaskQueue(queueCapacity);
    });
    
    builder.Services.AddSingleton<IPlatformDialog>(_ =>
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsDialog();
        }

        return new NullPlatformDialog();
    });

    builder.Services.AddRazorComponents().AddInteractiveServerComponents();

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

    builder.Services.AddDataContext(builder.Configuration);
    
    builder.Services.AddScoped<ITransactionManager, EfTransactionManager>();

    builder.Services.AddSingleton<IClock>(SystemClock.Instance);
    builder.Services.AddSingleton<DataSaveChangesInterceptor>();

    builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

    // Custom services.
    builder.Services.AddScoped<IInventoryService, InventoryService>();
    builder.Services.Decorate<IInventoryService, InventoryAlertDecorator>();
    builder.Services.Decorate<IInventoryService, InventoryLoggingDecorator>();
    builder.Services.AddScoped<ISupplierService, SupplierService>();
    builder.Services.Decorate<ISupplierService, SupplierLoggingDecorator>();
    builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
    builder.Services.Decorate<IPurchaseOrderService, PurchaseOrderLoggingDecorator>();
    builder.Services.AddScoped<IUomService, UomService>();
    builder.Services.Decorate<IUomService, UomLoggingDecorator>();
    builder.Services.AddSingleton<IAlertService, AlertService>();

    WebApplication app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();
    
    // Run Migrations.
    using (IServiceScope scope = app.Services.CreateScope())
    {
        DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
        await db.Database.MigrateAsync();
    }
    
    // Initialize Alert Service.
    await app.Services
        .GetRequiredService<IAlertService>()
        .InitializeAsync();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    
    if (OperatingSystem.IsWindows())
    {
        new WindowsDialog().ShowError(
            "Squrl Startup Error",
            ex.Message);
    }
}
finally
{
    Log.CloseAndFlush();
}