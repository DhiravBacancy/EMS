using EMS.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EMS.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(e => e.AdminId);

            builder.Property(e => e.AdminId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsRequired(true);

            builder.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsRequired(true);

            builder.Property(e => e.Email)
                .HasMaxLength(255)
                .IsRequired(true);

            builder.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsRequired(true);

            builder.Property(e => e.Role)
                .HasConversion<string>() // Store as string (VARCHAR)
                .HasMaxLength(20)        // Limit length to 20 (matching enum length)
                .IsRequired(true);

        }
    }
}
