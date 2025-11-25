using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.DTO.PaginationDtos;
using Expense_Tracker.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Expense_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseTrackerController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly ILogger<ExpenseTrackerController> _logger;

        public ExpenseTrackerController(IExpenseService expenseService, ILogger<ExpenseTrackerController> logger)
        {
            _expenseService = expenseService;
            _logger = logger;
        }
        private int GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid token. User ID not found.");
            }  
            return userId;
        }

        [Authorize(Roles = "User")]
        [HttpPost("CreateExpense")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDTO createExpenseDTO)
        {
            int userId = GetUserIdFromClaims();
            _logger.LogInformation("Creating expense for user {UserId}", userId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _logger.LogInformation("Model state is valid for user {UserId}", userId);
            var expense = await _expenseService.CreateExpenseAsync(createExpenseDTO, userId);
            _logger.LogInformation("Expense created successfully for user {UserId}", userId);
            if (expense == null)
                throw new ApplicationException("Expense creation failed."); // handled globally

            return CreatedAtAction(nameof(CreateExpense), new { id = expense.Id }, expense);
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetAllExpenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllExpenses([FromQuery] PaginationParams paginationParams)
        {
            int userId = GetUserIdFromClaims();
            var expense = await _expenseService.GetAllExpenses(userId, paginationParams);
            return Ok(expense);
        }

        [Authorize(Roles = "User")]
        [HttpPut("UpdateExpense/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseDto updateExpenseDto)
        {
            int userId = GetUserIdFromClaims();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var updatedExpense = await _expenseService.UpdateExpenseAsync(id, updateExpenseDto, userId);

            if (updatedExpense == null)
                throw new KeyNotFoundException($"Expense with ID {id} not found.");

            return Ok(updatedExpense);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("DeleteExpense/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            int userId = GetUserIdFromClaims();

            var isDeleted = await _expenseService.DeleteExpenseAsync(id, userId);

            if (!isDeleted)
                throw new KeyNotFoundException($"Expense with ID {id} not found.");

            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetExpense/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExpensebyIdAsync(int id)
        {
            int userId = GetUserIdFromClaims();
            var expense = await _expenseService.GetExpenseByIdAsync(id);

            if (expense == null)
                throw new KeyNotFoundException($"Expense with ID {id} not found.");

            return Ok(expense);
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetTotalExpenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTotalExpenses()
        {
            int userId = GetUserIdFromClaims();

            var totalExpenses = await _expenseService.GetTotalExpensesAsync(userId);

            return Ok(new TotalExpenseDto { TotalAmount = totalExpenses });
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetMonthlyExpenses")]
        [OutputCache(Duration = 600)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMonthlyExpenses()
        {
            int userId = GetUserIdFromClaims();

            var monthlyExpenses = await _expenseService.GetMonthlyExpnseAsync(userId);

            return Ok(monthlyExpenses);
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetSpendingByCategory")]
        [OutputCache(Duration = 600)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSpendingByCategory()
        {
            int userId = GetUserIdFromClaims();
            var categorySpendings = await _expenseService.GetSpendingByCategoryAsync(userId);
            return Ok(categorySpendings);
        }
    }
}
