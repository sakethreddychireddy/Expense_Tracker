using Expense_Tracker.DTO;
using Expense_Tracker.Models;

namespace Expense_Tracker.Service
{
    public interface IExpenseService
    {
        Task<User?> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<string> LoginAsync(LoginUserDto loginUserDto);
        Task<ExpenseDto?> CreateExpenseAsync(CreateExpenseDTO createExpenseDto,int userId);
        Task<IEnumerable<ExpenseDto>> GetAllExpenses(int userId);
        Task <ExpenseDto?> UpdateExpenseAsync(int id, UpdateExpenseDto updateExpenseDto,int userId);
        Task<bool> DeleteExpenseAsync(int id,int userId);
        Task<ExpenseDto> GetExpenseByIdAsync(int id);
        Task<bool> UserExistsAsync(string email);
        Task<decimal> GetTotalExpensesAsync(int userId);
        Task <List<MonthlyExpnseDto>> GetMonthlyExpenseAsync(int userId);
    }
}
