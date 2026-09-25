using Microsoft.Extensions.DependencyInjection;

namespace Squrl.App.Tests;

public abstract class IntegrationTest : IAsyncLifetime
{
    protected SqurlWebApplicationFactory Factory { get; }

    protected IntegrationTest(SqurlWebApplicationFactory factory)
    {
        Factory = factory;
    }

    public async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }
    
    protected IServiceScope CreateScope()
    {
        return Factory.CreateScope();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}