using Expense_Tracker.DTO.AuthDtos;
using Expense_Tracker.Models;

namespace Expense_Tracker.Service.Interfaces
{
    public interface IAuthServices
    {
        Task<User?> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<AuthResponseDto> LoginAsync(LoginUserDto loginUserDto);
        Task<bool> LogoutAsync(int userId, string refreshToken);
        Task<bool> UserExistsAsync(string email);
    }
}
