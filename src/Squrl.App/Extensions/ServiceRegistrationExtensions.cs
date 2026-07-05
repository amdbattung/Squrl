using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;

namespace Squrl.App.Extensions;

public static class ServiceRegistrationExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDataContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqurlContext");

            services.AddDbContextPool<DataContext>((sp, options) =>
            {
                options.UseSqlite(connectionString, o =>
                {
                    o.UseNodaTime();
                });
                options.UseSnakeCaseNamingConvention();
                options.AddInterceptors(sp.GetRequiredService<DataSaveChangesInterceptor>());
            });

            return services;
        }
    }
}