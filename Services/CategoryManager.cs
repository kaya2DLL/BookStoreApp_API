using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Eceptions.NotFoundException;

namespace Services
{
    public class CategoryManager : ICategoryService

    {
        private readonly IRepositoryManager _manager;

        public CategoryManager(IRepositoryManager manager)
        {
            _manager = manager;
        }

        public async Task<Category> CreateCategoryAsync(int id, bool trackChanges)
        {
            return await _manager.Category.GetOneCategoryByIdAsync(id, trackChanges);
        }

       

        public async Task DeleteOneCategoryAsync(int id, bool trackChanges)
        {
            var entity = await _manager.Category.GetOneCategoryByIdAsync(id, trackChanges);
             _manager.Category.DeleteOneCategory(entity);
            await _manager.SaveAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges)
        {
            return await _manager
                .Category
                .GetAllCategoriesAsync(trackChanges);
        }

        

        public async Task<Category> GetOneCategoryByIdAsync(int id, bool trackChanges)
        {
            var category = await _manager
             .Category
             .GetOneCategoryByIdAsync(id, trackChanges);

            if (category is null)
                throw new CategoryNotFoundException(id);
            return category;
        }
    }
}
