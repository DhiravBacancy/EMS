using EMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Reflection.Emit;

namespace EMS.Configurations
{

    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.EmployeeId);

            builder.Property(e => e.EmployeeId)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(e => e.Email)
                .IsUnique();

            builder.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(e => e.DateOfBirth)
                 .IsRequired(false);

            builder.Property(e => e.Address)
                .IsRequired(false);

            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Leaves)
                .WithOne(l => l.Employee)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(e => e.TechStack)
                .IsRequired(false);

            // Store the RoleEnum as String in the database
            builder.Property(e => e.Role)
                .HasConversion<string>() // Store as string (VARCHAR)
                .HasMaxLength(20)        // Limit length to 20 (matching enum length)
                .IsRequired(true);
        }
    }

}
