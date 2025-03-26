using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using EMS.DTOs;

namespace EMS.Service
{
    public interface IPdfService
    {
        byte[] GenerateEmployeeWorkHoursReport(EmployeeWorkReportDTO report, string reportType);
    }

    public class PdfService : IPdfService
    {
        public byte[] GenerateEmployeeWorkHoursReport(EmployeeWorkReportDTO report, string reportType)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header with Employee Details and Summary
                    page.Header()
                        .PaddingBottom(10)
                        .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                        .Column(col =>
                        {
                            col.Item().Text($"Employee Work Hours Report ({reportType.ToUpper()})")
                                .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2).AlignCenter();

                            col.Item().Text($"Employee Name: {report.EmployeeName}")
                                .FontSize(12).FontColor(Colors.Black);
                            col.Item().Text($"Email: {report.Email}")
                                .FontSize(12).FontColor(Colors.Black);
                            col.Item().Text($"Department: {report.Department}")
                                .FontSize(12).FontColor(Colors.Black);

                            // Summary row for work and leave details
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"Total Hours Worked: {report.TotalHoursWorked}")
                                    .FontSize(12).FontColor(Colors.Black);
                                row.RelativeItem().Text($"Leaves Taken: {report.LeavesTaken}")
                                    .FontSize(12).FontColor(Colors.Black);
                                row.RelativeItem().Text($"Paid Leaves Remaining: {report.PaidLeavesRemaining}")
                                    .FontSize(12).FontColor(Colors.Black);
                            });
                        });

                    // Content - Timesheet Table
                    page.Content()
                        .PaddingVertical(10)
                        .Table(table =>
                        {
                            // Define table columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30); // Index
                                columns.RelativeColumn(2);  // Date
                                columns.RelativeColumn(2);  // Start Time
                                columns.RelativeColumn(2);  // End Time
                                columns.RelativeColumn(2);  // Hours Worked
                                columns.RelativeColumn(3);  // Description
                            });

                            // Table Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("#").SemiBold();
                                header.Cell().Element(CellStyle).Text("Date").SemiBold();
                                header.Cell().Element(CellStyle).Text("Start Time").SemiBold();
                                header.Cell().Element(CellStyle).Text("End Time").SemiBold();
                                header.Cell().Element(CellStyle).Text("Hours Worked").SemiBold();
                                header.Cell().Element(CellStyle).Text("Description").SemiBold();
                            });

                            // Table Rows
                            int index = 1;
                            foreach (var timesheet in report.TimeSheets)
                            {
                                table.Cell().Element(CellStyle).Text(index++.ToString());
                                table.Cell().Element(CellStyle).Text(timesheet.Date.ToString("yyyy-MM-dd"));
                                table.Cell().Element(CellStyle).Text(timesheet.StartTime.ToString(@"hh\:mm"));
                                table.Cell().Element(CellStyle).Text(timesheet.EndTime.HasValue ? timesheet.EndTime.Value.ToString(@"hh\:mm") : "N/A");
                                table.Cell().Element(CellStyle).Text(timesheet.TotalHours.HasValue ? timesheet.TotalHours.Value.ToString("0.##") : "0");
                                table.Cell().Element(CellStyle).Text(string.IsNullOrWhiteSpace(timesheet.Description) ? "-" : timesheet.Description);
                            }

                            // Cell style helper
                            static IContainer CellStyle(IContainer container)
                            {
                                return container.Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        });

                    // Footer with timestamp
                    page.Footer()
                        .AlignRight()
                        .Text($"Generated on: {DateTime.Now:dd MMM yyyy HH:mm}")
                        .FontSize(10);
                });
            }).GeneratePdf();
        }
    }
}
