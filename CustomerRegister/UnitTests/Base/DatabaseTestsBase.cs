using System;
using System.IO;
using Database;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace UnitTests.Base
{
    public abstract class DatabaseTestsBase : IDisposable
    {
        protected readonly string DatabasePath;
        protected readonly IServiceProvider ServiceProvider;

        protected DatabaseTestsBase()
        {
            DatabasePath = Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid()}.sqlite");
            
            ServiceProvider = new ServiceCollection()
                .AddTestDatabase(DatabasePath)
                .AddLogging()
                .AddUnitOfWork()
                .AddRepositories()
                .AddServices()
                .BuildServiceProvider();
        }

        public virtual void Dispose()
        {
            if(File.Exists(DatabasePath)) File.Delete(DatabasePath);
        }
    }
}