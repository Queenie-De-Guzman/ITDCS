using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace MyApp.Models
{
    public class MusicModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(150)]
        public string Artist { get; set; }

        [MaxLength(150)]
        public string? Album { get; set; }

        [MaxLength(100)]
        public string? Genre { get; set; }

        [Range(1800, 2100, ErrorMessage = "Year must be between 1800 and 2100")]
        public int Year { get; set; }

        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
