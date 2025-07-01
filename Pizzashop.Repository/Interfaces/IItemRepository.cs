using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IItemRepository
{
    Task<MenuItem?> GetByIdAsync(int id);
    
    Task<IEnumerable<MenuItem>> GetAllByCategoryIdAsync(int categoryId);

    Task<(IEnumerable<MenuItem> list, int totalRecords )> GetPagedItemsAsync(int categoryId, string searchString, int page, int pagesize, bool isASC);

    Task AddAsync(MenuItem item);
    Task UpdateAsync(MenuItem item);
    Task UpdateRangeAsync(IEnumerable<MenuItem> items);
    Task AddRangeAsync(IEnumerable<MappingMenuItemWithModifier> items);
    Task RemoveRangeAsync(IEnumerable<MappingMenuItemWithModifier> items);
    Task UpdateRangeOfMapping(IEnumerable<MappingMenuItemWithModifier> items);
    Task<IEnumerable<MappingMenuItemWithModifier>> GetByItemId(int Id);
    Task<bool> IsExistsAsync(Expression<Func<MenuItem, bool>> predicate);
    Task<IEnumerable<MenuItem>> GetMenuItemByCategory(int categoryId, string searchString);
    Task<IEnumerable<MenuItem>> GetAvailableByCategoryIdAsync(int categoryId);
    Task<IEnumerable<MenuItem>> GetAllAvailableAsync();
    Task<IEnumerable<MenuItem>> GetFavoriteItemsAsync();
}
