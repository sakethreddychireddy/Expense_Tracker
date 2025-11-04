using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = "User"; // default role

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<RecurringExpense> RecurringExpenses { get; set; } = new List<RecurringExpense>();
        public ICollection<Categories> Categories { get; set; } = new List<Categories>();
    }
}
