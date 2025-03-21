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

            builder.HasOne(e => e.Employee)
               .WithMany(l => l.Leaves)
               .HasForeignKey(l => l.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Property(l => l.StartDate)
                .IsRequired(true);

            builder.Property(l => l.EndDate)
                .IsRequired(true);

            builder.Property(l => l.Reason)
                .HasMaxLength(255)
                .IsRequired(false);

            // Store StatusEnum as String (VARCHAR)
            builder.Property(l => l.Status)
                .HasConversion<string>() // Store enum as string
                .HasMaxLength(20)
                .IsRequired(); // NOT NULL

            // Store LeaveTypeEnum as String (VARCHAR)
            builder.Property(l => l.LeaveType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

        }
    }
}
