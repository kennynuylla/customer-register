using System;
using System.IO;
using System.Linq;
using Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WebAPI;

namespace IntegrationTests
{
    public class ApplicationFactory : WebApplicationFactory<Startup>
    {
        public string DatabasePath { get; }

        public ApplicationFactory()
        {
            DatabasePath = Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid()}.sqlite");
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(serviceCollection =>
            {
                var contextDescriptor = serviceCollection.Single(x => x.ServiceType == typeof(ApplicationContext));
                serviceCollection.Remove(contextDescriptor);

                serviceCollection.AddTestDatabase(DatabasePath);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if(File.Exists(DatabasePath)) File.Delete(DatabasePath);
        }
    }
}