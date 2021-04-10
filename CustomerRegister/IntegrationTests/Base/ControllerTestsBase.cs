using System;
using System.Text.Json;
using Database;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests.Base
{
    public abstract class ControllerTestsBase : IClassFixture<ApplicationFactory>, IDisposable
    {
        protected readonly ApplicationFactory Factory;
        protected readonly IServiceProvider ServiceProvider;
        
        protected ControllerTestsBase(ApplicationFactory factory)
        {
            Factory = factory;
            ServiceProvider = new ServiceCollection()
                .AddTestDatabase(Factory.DatabasePath)
                .AddSingleton(sp => new JsonSerializerOptions {PropertyNameCaseInsensitive = true})
                .BuildServiceProvider();
        }
        public void Dispose()
        {
            Factory?.Dispose();
        }
    }
}