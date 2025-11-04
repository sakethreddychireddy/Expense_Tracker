namespace Expense_Tracker.DTO.AuthDtos
{
    public class LogoutDto
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
