using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using EMS.DTOs;


namespace EMS.Service
{
    public interface IPdfService
    {
        byte[] GenerateEmployeeWorkHoursReport(EmployeeDetailsDTO employee, string reportType);
    }
    public class PdfService : IPdfService
    {
        public byte[] GenerateEmployeeWorkHoursReport(EmployeeDetailsDTO employee, string reportType)
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

                    // Header with Employee Details
                    page.Header().Column(column =>
                    {
                        column.Item().Text($"Employee Work Hours Report ({reportType})")
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                        column.Item().Text($"Employee Name: {employee.FullName}").FontSize(12);
                        column.Item().Text($"Employee ID: {employee.EmployeeId}").FontSize(12);
                        column.Item().Text($"Email: {employee.Email}").FontSize(12);
                        column.Item().Text($"Phone: {employee.Phone}").FontSize(12);
                        column.Item().Text($"Department: {employee.Department}").FontSize(12);
                        column.Item().Text($"Tech Stack: {employee.TechStack}").FontSize(12);
                    });

                    // Work Hours Table
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50); // Index
                            columns.RelativeColumn();  // Date
                            columns.RelativeColumn();  // Work Hours
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("#").Bold();
                            header.Cell().Text("Date").Bold();
                            header.Cell().Text("Work Hours").Bold();
                        });

                        int index = 1;
                        foreach (var entry in employee.TimeSheets)
                        {
                            table.Cell().Text(index++.ToString());
                            table.Cell().Text(entry.Date.ToString("yyyy-MM-dd"));
                            table.Cell().Text(entry.HoursWorked.ToString());
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
