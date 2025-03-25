using EMS.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EMS.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.DepartmentId);

            builder.Property(d => d.DepartmentId)
                .ValueGeneratedOnAdd()
                .IsRequired(true);

            builder.Property(d => d.DepartmentName)
                .HasColumnType("VARCHAR(100)")
                .IsRequired(true);

            // Department - Employee relationship (One-to-Many)
            builder.HasMany(d => d.Employees)
                .WithOne(e => e.Department) // Assuming the Employee model has a navigation property 'Department'
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull); // Set to NULL if the department is deleted
        }
    }
}
