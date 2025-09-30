using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.DTO
{
    public class CreateExpenseDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public string Category { get; set; } = string.Empty;
    }
}
