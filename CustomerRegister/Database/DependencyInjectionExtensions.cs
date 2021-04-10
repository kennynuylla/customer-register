using Database.Repositories;
using Database.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Repositories;

namespace Database
{
    public static class DependencyInjectionExtensions
    {

        public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            using var context = serviceProvider.GetRequiredService<ApplicationContext>();
            context.Database.Migrate();

            return serviceCollection;
        }

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
            serviceCollection
                .AddScoped<IAddressRepository, AddressRepository>()
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IPhoneRepository, PhoneRepository>()
                .AddScoped<ILocalPhoneRepository, LocalPhoneRepository>();
            return serviceCollection;
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            return serviceCollection;
        }
    }
}