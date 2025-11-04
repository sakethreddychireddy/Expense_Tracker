using System.Globalization;

namespace Expense_Tracker.DTO.AuthDtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int UserId { get; set; } 
    }
}
