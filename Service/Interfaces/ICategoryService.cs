using Expense_Tracker.DTO.CategoryDtos;

namespace Expense_Tracker.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto > CreateCategoryAsync(CreateCategoryDto dto,int userId);
        //Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto,int userId);
        Task<IEnumerable<CategoryResponseDto>> GetUserCategoriesAsync();
        //Task<bool> DeleteCategoryAsync(int id,int userId);
        //Task<CategoryResponseDto?> GetCategoryByIdAsync(int id,int userId);

    }
}
