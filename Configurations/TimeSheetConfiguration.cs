using EMS.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EMS.Configurations
{
    public class TimeSheetConfiguration : IEntityTypeConfiguration<TimeSheet>
    {
        public void Configure(EntityTypeBuilder<TimeSheet> builder)
        {
            builder.HasKey(ts => ts.TimesheetId);
            
            builder.Property(ts => ts.TimesheetId)
                .ValueGeneratedOnAdd()
                .IsRequired(true);

            // TimeSheet - Employee relationship (Many-to-One)
            builder.HasOne(ts => ts.Employee) // Assuming the TimeSheet model has a navigation property 'Employee'
                .WithMany(e => e.TimeSheets) // Assuming the Employee model has a navigation property 'TimeSheets'
                .HasForeignKey(ts => ts.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade); // Delete related timesheets if the employee is deleted

            builder.Property(ts => ts.Date)
                .IsRequired(true);

            builder.Property(ts => ts.StartTime)
                .HasColumnType("TIME")
                .IsRequired(true);

            builder.Property(ts => ts.EndTime)
                .HasColumnType("TIME")
               .IsRequired(true);

            builder.Property(ts => ts.Description)
                .HasColumnType("TEXT")
                .IsRequired(false);


        }
    }
}
