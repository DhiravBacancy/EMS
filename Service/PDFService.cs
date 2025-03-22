using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace EMS.Service
{
    public interface IPDFService 
    {
        Task<byte[]> GeneratePDFAsync(string PDFContent);

    }
    public class PDFService : IPDFService
    {
        public async Task<byte[]> GeneratePDFAsync(string PDFContent)
        {
            QuestPDF.Settings.License = LicenseType.Community; 

            var pdfDocument = Document.Create(container =>  
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Content().PaddingVertical(10).Text(PDFContent).FontSize(12);
                    page.Footer().AlignRight().Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}");
                });
            });

            using var stream = new MemoryStream();
            pdfDocument.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}
