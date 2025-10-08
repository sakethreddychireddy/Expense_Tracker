using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class RecurringExpense
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public string Frequency { get; set; } = "Monthly";
        // e.g., Daily, Weekly, Monthly, Yearly
        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }  // Optional stop date

        public DateTime NextOccurrence { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // User relationship
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
