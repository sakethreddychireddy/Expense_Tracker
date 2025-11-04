using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Categories
    {
        [Key]
        public int Id { get; set; }
        [Required,MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(255)]
        public string? Description { get; set; }

        //public int UserId { get; set; }

        //[ForeignKey(nameof(UserId))]
        //public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
    }
}
