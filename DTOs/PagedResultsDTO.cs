using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    public class PagedResultDTO<T>
    {
        [Required]
        public IEnumerable<T> Items { get; set; } = new List<T>();
        [Required]
        public int TotalRecords { get; set; }
        [Required]
        public int PageNumber { get; set; }
        [Required]
        public int PageSize { get; set; }
    }

}
