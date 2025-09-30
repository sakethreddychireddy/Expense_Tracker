using Expense_Tracker.Data;
using Expense_Tracker.DTO;
using Expense_Tracker.Models;
using Expense_Tracker.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Expense_Tracker.ServiceImpl
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExpenseService> _logger;
        private readonly IJwtService _jwtService;
        public ExpenseService(AppDbContext context, ILogger<ExpenseService> logger, IJwtService jwtService)
        {
            _context = context;
            _logger = logger;
            _jwtService = jwtService;
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
                    Category = createExpenseDto.Category,
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
                    Category = expense.Category
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
                    .Select(e => new ExpenseDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Amount = e.Amount,
                        Date = e.Date,
                        Category = e.Category
                    })
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
            expense.Category = updateExpenseDto.Category;

            await _context.SaveChangesAsync();

            return new ExpenseDto
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                Date = expense.Date,
                Category = expense.Category
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
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
            if (expense is null)
            {
                throw new KeyNotFoundException($"Expense with ID {id} not found");
            }
            var expenseDto = new ExpenseDto
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                Date = expense.Date,
                Category = expense.Category
            };
            return expenseDto;
        }
        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<User?> RegisterUserAsync(RegisterUserDto dto)
        {
            try
            {
                var user = new User
                {
                    Email = dto.Email,
                    Role = "User"
                };
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {Email} registered successfully.", user.Email);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user.");
                throw;
            }
        }

        public async Task<string> LoginAsync(LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginUserDto.Email);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: No user found with email {Email}", loginUserDto.Email);
                    // Return empty string to match non-nullable return type
                    return string.Empty;
                }

                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning("Login failed: Incorrect password for email {Email}", loginUserDto.Email);
                    // Return empty string to match non-nullable return type
                    return string.Empty;
                }

                var token = _jwtService.GenerateToken(user);

                _logger.LogInformation("User {Email} logged in successfully.", user.Email);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login.");
                throw new Exception($"Error during login: {ex.Message}", ex);
            }
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
                    }).ToListAsync();

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
    }
}
