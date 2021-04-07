using Domain.Enums;
using Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations.Base
{
    internal abstract class PhoneConfigurationBase<TPhone>: IEntityTypeConfiguration<TPhone> where TPhone: PhoneBase
    {
        public void Configure(EntityTypeBuilder<TPhone> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Number).IsRequired();
            builder.Property(x => x.AreaCode).IsRequired();
            builder.Property(x => x.Type).HasConversion(x => (int) x, x => (PhoneType) x);
        }
    }
}