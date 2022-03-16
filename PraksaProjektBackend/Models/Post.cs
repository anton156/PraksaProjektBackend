using PraksaProjektBackend.Filter;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PraksaProjektBackend.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [NotMapped]
        public IFormFile? PostImage { get; set; }
        [SwaggerIgnore]
        public string? ImagePath { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }
        [SwaggerIgnore]
        public DateTime? CreatedDate { get; set; }
    }
}
