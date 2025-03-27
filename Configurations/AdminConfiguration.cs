using EMS.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EMS.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(a => a.AdminId);

            builder.Property(a => a.AdminId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(a => a.FirstName)
                .HasColumnType("VARCHAR(100)")
                .IsRequired(true);

            builder.Property(a => a.LastName)
                .HasColumnType("VARCHAR(100)")
                .IsRequired(true);

            builder.Property(a => a.Email)
                .HasColumnType("VARCHAR(255)")
                .IsRequired(true);

            builder.HasIndex(a => a.Email)
                .IsUnique(true);

            builder.Property(a => a.Phone)
                .HasColumnType("VARCHAR(15)")
                .IsRequired(true);

            builder.Property(a => a.Role)
                .HasConversion<string>()
                .HasMaxLength(20)      
                .IsRequired(true);

        }
    }
}
