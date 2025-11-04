using Expense_Tracker.Data;
using Expense_Tracker.DTO.CategoryDtos;
using Expense_Tracker.Models;
using Expense_Tracker.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Service.Implementations
{
    public class CategoryServices : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryServices> _logger;
        private readonly IJwtService _jwtService;
        public CategoryServices(AppDbContext context, ILogger<CategoryServices> logger, IJwtService jwtService)
        {
            _context = context;
            _logger = logger;
            _jwtService = jwtService;
        }
        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto,int userId)
        {
            var category = new Categories
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt
            };
        }
        public async Task<IEnumerable<CategoryResponseDto>> GetUserCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt
            });
        }
    }
}
