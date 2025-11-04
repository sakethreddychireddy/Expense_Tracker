using Expense_Tracker.DTO.CategoryDtos;
using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.Models;

namespace Expense_Tracker.Service.Interfaces
{
    public interface IExpenseService
    {
        //Task<User?> RegisterUserAsync(RegisterUserDto registerUserDto);
        //Task<AuthResponseDto> LoginAsync(LoginUserDto loginUserDto);
        //Task<bool> LogoutAsync(int userId,string refreshToken);
        Task<ExpenseDto?> CreateExpenseAsync(CreateExpenseDTO createExpenseDto,int userId);
        Task<IEnumerable<ExpenseDto>> GetAllExpenses(int userId);
        Task <ExpenseDto?> UpdateExpenseAsync(int id, UpdateExpenseDto updateExpenseDto,int userId);
        Task<bool> DeleteExpenseAsync(int id,int userId);
        Task<ExpenseDto> GetExpenseByIdAsync(int id);
        //<bool> UserExistsAsync(string email);
        Task<decimal> GetTotalExpensesAsync(int userId);
        Task <List<MonthlyExpnseDto>> GetMonthlyExpenseAsync(int userId);
        Task<List<CategorySpendingDto>> GetSpendingByCategoryAsync(int userId);
    }
}
