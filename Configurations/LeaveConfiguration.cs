using EMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMS.Configurations
{
    public class LeaveConfiguration : IEntityTypeConfiguration<Leave>
    {
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            builder.HasKey(l => l.LeaveId);

            builder.Property(l => l.LeaveId)
                .ValueGeneratedOnAdd()
                .IsRequired(true);

            // Leave - Employee relationship (Many-to-One)
            builder.HasOne(l => l.Employee) // Assuming the Leave model has a navigation property 'Employee'
                .WithMany(e => e.Leaves) // Assuming the Employee model has a navigation property 'Leaves'
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade); // Delete related leaves if the employee is deleted

            builder.Property(l => l.StartDate)
                .IsRequired(true);

            builder.Property(l => l.EndDate)
                .IsRequired(true);

            builder.Property(l => l.Reason)
                .HasColumnType("TEXT")
                .IsRequired(false);

            // Store StatusEnum as String (VARCHAR)
            builder.Property(l => l.Status)
                .HasConversion<string>() // Store enum as string
                .HasColumnType("VARCHAR(20)")
                .IsRequired(); // NOT NULL

            // Store LeaveTypeEnum as String (VARCHAR)
            builder.Property(l => l.LeaveType)
                .HasConversion<string>()
                .HasColumnType("VARCHAR(50)")
                .IsRequired();
        }
    }
}
