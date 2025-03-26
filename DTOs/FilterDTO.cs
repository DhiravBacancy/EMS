using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    // In EMS.DTOs
    public class FilterDTO
    {
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public object Value { get; set; }  // Object type to handle any data type

        public string Operator { get; set; } = "="; // Default to equality
    }
}
