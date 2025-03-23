using System.ComponentModel.DataAnnotations;
using EMS.Helpers;
namespace EMS.DTOs
{
    public class EmailMessageDTO
    {
        [Required]
        [ValidEmailHelper]
        public string ToEmail { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        [Required]
        public bool IsHtml { get; set; } = true; // Default to HTML format
    }
}
