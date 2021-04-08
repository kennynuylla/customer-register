using System.Linq;
using Database.Configurations;
using Domain.Models;
using Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            optionsBuilder.UseSqlServer("Server=127.0.0.1,1450;Database=ilia;User Id=SA;Password=Develop123456*;");
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
                .Where(x => !x.IsAbstract && !x.IsInterface && x.GetInterfaces().Contains(typeof(IUuidModel)));

            foreach (var haveId in iHaveIds)
            {
                modelBuilder.Entity(haveId, builder =>
                {
                    builder.HasIndex(nameof(IUuidModel.Uuid)).IsUnique();
                    builder.Property(nameof(IUuidModel.Uuid)).HasDefaultValueSql("newid()");
                });
            }
        }
    }
}