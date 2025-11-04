using Expense_Tracker.Models;

namespace Expense_Tracker.Service.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User? user);
        string GenerateRefreshToken();

    }
}
