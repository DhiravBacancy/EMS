using EMS.Enums;
using EMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

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

            builder.Property(l => l.Status)
                .HasConversion(
                    v => v.ToString(), // Converts the enum to string for storage in JSON
                    v => (StatusEnum)Enum.Parse(typeof(StatusEnum), v) // Converts string back to enum when deserializing
                ).IsRequired(); // NOT NULL

            builder.Property(l => l.LeaveType)
                .HasConversion(
                     v => v.ToString(), // Converts the enum to string for storage in JSON
                    v => (LeaveTypeEnum)Enum.Parse(typeof(LeaveTypeEnum), v) // Converts string back to enum when deserializing
                ).IsRequired();
        }
    }
}
