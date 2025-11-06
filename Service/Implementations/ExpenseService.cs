using Expense_Tracker.Data;
using Expense_Tracker.DTO.CategoryDtos;
using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.Models;
using Expense_Tracker.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Expense_Tracker.Service.Implementations
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExpenseService> _logger;
        public ExpenseService(AppDbContext context, ILogger<ExpenseService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ExpenseDto?> CreateExpenseAsync(CreateExpenseDTO createExpenseDto, int userId)
        {
            try
            {
                // Make sure the user exists
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found. Cannot create expense.", userId);
                    return null;
                }

                var expense = new Expense
                {
                    Title = createExpenseDto.Title,
                    Amount = createExpenseDto.Amount,
                    Date = createExpenseDto.Date,
                    CategoryId = createExpenseDto.CategoryId,
                    UserId = userId // link expense to user
                };

                await _context.Expenses.AddAsync(expense);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Expense with ID {ExpenseId} created successfully for User {UserId}.", expense.Id, userId);

                return new ExpenseDto
                {
                    Id = expense.Id,
                    Title = expense.Title,
                    Amount = expense.Amount,
                    Date = expense.Date,
                    //Category = expense.Category
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense for User {UserId}.", userId);
                throw;
            }
        }
        public async Task<IEnumerable<ExpenseDto>> GetAllExpenses(int userId)
        {
            try
            {
                _logger.LogInformation("Retrieving all expenses.");
                var expenses = await _context.Expenses.Where(e => e.UserId == userId)
                    .Include(e => e.Category)
                    .Select(e => new ExpenseDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Amount = e.Amount,
                        Date = e.Date,
                        CategoryName = e.Category.Name
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return expenses;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving expenses: {ex.Message}", ex);
                throw new ApplicationException("Failed to retrieve expenses.", ex);
            }
        }
        public async Task<ExpenseDto?> UpdateExpenseAsync(int id, UpdateExpenseDto updateExpenseDto, int userId)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null)
                return null;

            if (expense.UserId != userId)
                throw new UnauthorizedAccessException("You cannot update another user's expense.");

            expense.Title = updateExpenseDto.Title;
            expense.Amount = updateExpenseDto.Amount;
            expense.Date = updateExpenseDto.Date;
            expense.CategoryId = updateExpenseDto.CategoryId;

            await _context.SaveChangesAsync();
            await _context.Entry(expense).Reference(e => e.Category).LoadAsync();

            return new ExpenseDto
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                Date = expense.Date,
                CategoryName = expense.Category.Name
            };
        }

        public async Task<bool> DeleteExpenseAsync(int id, int userId)
        {
            _logger.LogInformation($"Delete expense");
            var expense = await _context.Expenses.FindAsync(id);
            if (expense is null)
            {
                throw new Exception($"Expense with ID {id} not found.");
            }
            if (expense.UserId != userId)
            {
                throw new UnauthorizedAccessException("You cannot delete another user's expense.");
            }
            try
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting expense: {ex.Message}", ex);
            }
        }
        public async Task<ExpenseDto> GetExpenseByIdAsync(int id)
        {
            var expense = await _context.Expenses.Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (expense is null)
            {
                throw new KeyNotFoundException($"Expense with ID {id} not found");
            }
            return new ExpenseDto
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                Date = expense.Date,
                CategoryName = expense.Category.Name,
                CategoryId = expense.Category.Id,
            };
        }
        public async Task<decimal> GetTotalExpensesAsync(int userId)
        {
            try
            {
                var total = await _context.Expenses
                    .Where(e => e.UserId == userId)
                    .SumAsync(e => e.Amount);
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total expenses for User {UserId}.", userId);
                throw new Exception($"Error calculating total expenses: {ex.Message}", ex);
            }
        }
        public async Task<List<MonthlyExpnseDto>> GetMonthlyExpenseAsync(int userId)
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12)
                .Select(m => new MonthlyExpnseDto
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m),
                    TotalAmount = 0
                }).ToList();

                var expenses = await _context.Expenses
                    .Where(e => e.UserId == userId)
                    .GroupBy(e => e.Date.Month)
                    .Select(g => new MonthlyExpnseDto
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                        TotalAmount = g.Sum(e => e.Amount)
                    }).AsNoTracking()
                    .ToListAsync();

                // Merge so months with no expenses show 0
                foreach (var exp in expenses)
                {
                    var month = allMonths.First(m => m.Month == exp.Month);
                    month.TotalAmount = exp.TotalAmount;
                }
                return allMonths;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly expenses.");
                throw new Exception($"Error retrieving monthly expenses: {ex.Message}", ex);
            }
        }
        public async Task<List<CategorySpendingDto>> GetSpendingByCategoryAsync(int userId)
        {
            try
            {                
                var spendingByCategory = await _context.Database
                    .SqlQuery<CategorySpendingDto>($"EXEC getCategorySpendingsByUser @UserId = {userId}")
                    .AsNoTracking()
                    .ToListAsync();
                return spendingByCategory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving spending by category for User {UserId}.", userId);
                throw new Exception($"Error retrieving spending by category: {ex.Message}", ex);
            }
        }
    }
}
