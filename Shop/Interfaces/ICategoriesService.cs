using Exam2.Backend.Entities;

namespace Shop.Interfaces
{
    public interface ICategoriesService
    {
        IEnumerable<Category> GetAllCategories();
        Category? GetCategoryById(int id);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int id);
    }
}
