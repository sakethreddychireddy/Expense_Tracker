using Expense_Tracker.Data;
using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Expense_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseTrackerController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly AppDbContext _context;
        private readonly ILogger<ExpenseTrackerController> _logger;

        public ExpenseTrackerController(IExpenseService expenseService, AppDbContext context, ILogger<ExpenseTrackerController> logger)
        {
            _expenseService = expenseService;
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "User")]
        [HttpPost("CreateExpense")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDTO createExpenseDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token. User ID not found.");

            var expense = await _expenseService.CreateExpenseAsync(createExpenseDTO, userId);

            if (expense == null)
                throw new ApplicationException("Expense creation failed."); // handled globally

            return CreatedAtAction(nameof(CreateExpense), new { id = expense.Id }, expense);
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetAllExpenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllExpenses()
        {
            var expenses = await _expenseService.GetAllExpenses(
                int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value ?? "0"));

            return Ok(expenses);
        }

        [Authorize(Roles = "User")]
        [HttpPut("UpdateExpense/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseDto updateExpenseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token. User ID not found.");

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token. User ID not found.");

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token. User ID not found.");

            var totalExpenses = await _expenseService.GetTotalExpensesAsync(userId);

            return Ok(new TotalExpenseDto { TotalAmount = totalExpenses });
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetMonthlyExpenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMonthlyExpenses()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token. User ID not found.");

            var monthlyExpenses = await _expenseService.GetMonthlyExpenseAsync(userId);

            return Ok(monthlyExpenses);
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetSpendingByCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSpendingByCategory()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token. User ID not found.");
            var categorySpendings = await _expenseService.GetSpendingByCategoryAsync(userId);
            return Ok(categorySpendings);
        }
    }
}
