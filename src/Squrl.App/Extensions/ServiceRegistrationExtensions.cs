using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Squrl.App.Data;
using Squrl.App.Infrastructure.DataExporter;
using Squrl.App.Infrastructure.NodaTimePersistentStateSerializer;

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

        public IServiceCollection AddCsvExportersFromAssembly(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            IEnumerable<Type> exporterTypes = assembly
                .GetTypes()
                .Where(type =>
                    type is { IsClass: true, IsAbstract: false } &&
                    !type.ContainsGenericParameters);

            foreach (Type implementationType in exporterTypes)
            {
                Type? exporterBaseType = implementationType.BaseType;
                
                while (exporterBaseType is not null &&
                       (!exporterBaseType.IsGenericType ||
                        exporterBaseType.GetGenericTypeDefinition() != typeof(CsvExporter<>)))
                {
                    exporterBaseType = exporterBaseType.BaseType;
                }

                if (exporterBaseType is null)
                {
                    continue;
                }

                Type modelType = exporterBaseType.GetGenericArguments()[0];

                Type serviceType = typeof(ICsvExporter<>)
                    .MakeGenericType(modelType);

                services.AddTransient(serviceType, implementationType);
            }

            return services;
        }

        public IServiceCollection AddNodaTimePersistentState<T>()
        {
            services.AddSingleton<
                PersistentComponentStateSerializer<T>,
                NodaTimePersistentStateSerializer<T>>();

            return services;
        }
    }
}