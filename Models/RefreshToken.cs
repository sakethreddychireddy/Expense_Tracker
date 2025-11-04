using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
