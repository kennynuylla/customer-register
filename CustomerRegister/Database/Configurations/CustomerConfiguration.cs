using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .HasIndex(x => x.Email)
                .IsUnique();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.Name).IsRequired();
        }
    }
}