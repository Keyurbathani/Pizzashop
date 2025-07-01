using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ICategoryRepository
{
    Task<MenuCategory?> GetByIdAsync(int id);
    Task<IEnumerable<MenuCategory>> GetAllAsync();
    Task AddAsync(MenuCategory category);
    Task UpdateAsync(MenuCategory category);
    Task<bool> IsExistsAsync(Expression<Func<MenuCategory, bool>> predicate);

}
