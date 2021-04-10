using System.Linq;
using Database.Configurations;
using Domain.Models;
using Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<LocalPhone> LocalPhones { get; set; }
        public DbSet<Phone> Phones { get; set; }

        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            
            var assemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            var pathWithoutFilePrefix = assemblyPath.Substring(6);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(pathWithoutFilePrefix)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("database"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AddressConfiguration())
                .ApplyConfiguration(new CustomerConfiguration())
                .ApplyConfiguration(new PhoneConfiguration())
                .ApplyConfiguration(new LocalPhoneConfiguration());
            
            ConfigureUuids(modelBuilder);
        }

        private void ConfigureUuids(ModelBuilder modelBuilder)
        {
            if (Database.IsSqlite()) return;

            var iHaveIds = modelBuilder.Model.GetEntityTypes()
                .Select(x => x.ClrType)
                .Where(x => !x.IsAbstract && !x.IsInterface && x.GetInterfaces().Contains(typeof(IBaseModel)));

            foreach (var haveId in iHaveIds)
            {
                modelBuilder.Entity(haveId, builder =>
                {
                    builder.HasIndex(nameof(IBaseModel.Uuid)).IsUnique();
                    builder.Property(nameof(IBaseModel.Uuid)).HasDefaultValueSql("newid()");
                });
            }
        }
    }
}