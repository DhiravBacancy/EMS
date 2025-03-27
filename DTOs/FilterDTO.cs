using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    public class FilterDTO
    {
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public object Value { get; set; }

        public string Operator { get; set; } = "="; 
    }
}
