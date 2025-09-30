using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.DTO
{
    public class UpdateExpenseDto
    {
        public int Id { get; set; } 
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;
    }
}
