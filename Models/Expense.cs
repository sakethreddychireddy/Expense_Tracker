using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [Required]
        public string Category { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
