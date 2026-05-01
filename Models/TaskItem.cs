using System.ComponentModel.DataAnnotations;

namespace DeveloprBud.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } // not a required field
        public string? Priority { get; set; } // not a required field
        public string? Tag { get; set; } // not a required field
        public string? Status { get; set; } // status default to "not completed"
        public DateTime CreatedDate { get; set; } // defalt date set
        public DateTime? CompletedDate { get; set; }
        public string? UserId { get; set; } = string.Empty; // foreign key to associate task with a user
    }
}
