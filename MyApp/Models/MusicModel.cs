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
        public string Title { get; set; }

        [Required]
        public string Artist { get; set; }

        public string Album { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }

        public string? ImagePath { get; set; }
        public string? AudioPath { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public IFormFile? AudioFile { get; set; }
    }
}
