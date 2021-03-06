using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Services.Services;
using Services.Services.Interfaces;

namespace Services
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddScoped<IAddressService, AddressService>()
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<IPhoneService, PhoneService>()
                .AddScoped<ILocalPhoneService, LocalPhoneService>();
            
            return serviceCollection;
        }
    }
}