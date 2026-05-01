using System.ComponentModel.DataAnnotations;

namespace DeveloprBud.Models
{
    public class CodeSnippet
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Code { get; set; } = string.Empty;
        public string? Language { get; set; } // ? / not a required field
        public string? Notes { get; set; } // code context - notes / ? // not a required field
        public DateTime CreatedDate { get; set; } // default date set
        public string? UserId { get; set; } = string.Empty; // foreign key to associate code snippet with a user
    }
}