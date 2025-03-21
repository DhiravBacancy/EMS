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

            builder.HasOne(ts => ts.Employee)
                .WithMany(e => e.TimeSheets)
                .HasForeignKey(ts => ts.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ts => ts.Date)
                .IsRequired(true);

            builder.Property(ts => ts.StartTime)
                .IsRequired(true);

            builder.Property(ts => ts.EndTime)
               .IsRequired(true);

            builder.Property(ts => ts.TotalHours)
                .IsRequired(true);

            builder.Property(ts => ts.Description)
                .IsRequired(false);


        }
    }
}
