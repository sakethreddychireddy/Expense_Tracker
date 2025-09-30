using Expense_Tracker.Models;

namespace Expense_Tracker.Service
{
    public interface IJwtService
    {
        string GenerateToken(User? user);
    }
}
