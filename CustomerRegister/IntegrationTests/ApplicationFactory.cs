using System;
using System.IO;
using System.Linq;
using Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAPI;

namespace IntegrationTests
{
    public class ApplicationFactory : WebApplicationFactory<FixtureStartup>
    {
        public string DatabasePath { get; }

        public ApplicationFactory()
        {
            DatabasePath = Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid()}.sqlite");
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(x =>
                {
                    x.UseStartup<FixtureStartup>().UseTestServer();
                });
            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddTestDatabase(DatabasePath);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if(File.Exists(DatabasePath)) File.Delete(DatabasePath);
        }
    }
}