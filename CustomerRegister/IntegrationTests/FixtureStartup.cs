using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAPI;

namespace IntegrationTests
{
    public class FixtureStartup : Startup
    {
        public FixtureStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void AddDatabase(IServiceCollection services)
        {
        }
    }
}