using Entities.Models;

namespace Services.Contracts
{
    public interface ICategoryService 
    { 
        Task<IEnumerable<Category>>GetAllCategoriesAsync(bool trackChanges);
        Task<Category> GetOneCategoryByIdAsync(int id,bool trackChanges);

        Task<Category> CreateCategoryAsync(int id, bool trackChanges);

        Task DeleteOneCategoryAsync(int id, bool trackChanges);
       
    }
}
