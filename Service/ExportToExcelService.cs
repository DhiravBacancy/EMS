using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMS.Models;
using OfficeOpenXml;
using System.IO;

namespace EMS.Services
{
    public interface IExportTimesheetsToExcelService
    {
        Task<FileContentResult> ExportToExcel(List<TimeSheet> timesheets);
    }

    public class ExportTimesheetsToExcelService : IExportTimesheetsToExcelService
    {
        public async Task<FileContentResult> ExportToExcel(List<TimeSheet> timesheets)
        {
            try
            {
                // Set the License Context to NonCommercial (if using EPPlus under the free license)
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Timesheets");

                    if (timesheets == null || timesheets.Count == 0)
                        throw new Exception("No timesheet data available.");

                    worksheet.Cells[1, 1].Value = "Date";
                    worksheet.Cells[1, 2].Value = "Start Time";
                    worksheet.Cells[1, 3].Value = "End Time";
                    worksheet.Cells[1, 4].Value = "Total Hours";
                    worksheet.Cells[1, 5].Value = "Description";

                    for (int i = 0; i < timesheets.Count; i++)
                    {
                        var t = timesheets[i];
                        worksheet.Cells[i + 2, 1].Value = t.Date.ToString("yyyy-MM-dd");
                        worksheet.Cells[i + 2, 2].Value = t.StartTime.ToString();
                        worksheet.Cells[i + 2, 3].Value = t.EndTime?.ToString();
                        worksheet.Cells[i + 2, 4].Value = t.TotalHours.HasValue ? t.TotalHours.Value.ToString("F2") : "N/A";
                        worksheet.Cells[i + 2, 5].Value = t.Description;
                    }

                    worksheet.Cells.AutoFitColumns();

                    var fileBytes = await Task.Run(() => package.GetAsByteArray());


                    return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = "Timesheets.xlsx"
                    };
                }
            }
            catch (Exception ex)
            {
                // Log and handle exception as needed
                Console.WriteLine($"Error generating Excel file: {ex.Message}");
                throw;
            }
        }


    }
}
