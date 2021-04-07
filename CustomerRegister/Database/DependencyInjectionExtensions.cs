using Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Repositories;

namespace Database
{
    public static class DependencyInjectionExtensions
    {

        public static IServiceCollection AddTestDatabase(this IServiceCollection serviceCollection, string databasePath)
        {
            serviceCollection.AddDbContext<ApplicationContext>(options =>
            {
                options
                    .EnableSensitiveDataLogging()
                    .UseSqlite($"Filename={databasePath}");
            });

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            using var context = serviceProvider.GetRequiredService<ApplicationContext>();
            context.Database.EnsureCreated();
            
            return serviceCollection;
        }
        
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAddressRepository, AddressRepository>();

            return serviceCollection;
        }
    }
}