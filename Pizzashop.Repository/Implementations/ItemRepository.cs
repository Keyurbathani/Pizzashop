using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class ItemRepository : IItemRepository
{
    private readonly PizzaShopContext _context;

    public ItemRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MenuItem>> GetAllByCategoryIdAsync(int categoryId)
    {
        if(categoryId == 0){
        return await _context.MenuItems.Where(i =>!(bool)i.IsDeleted!).OrderBy(i => i.Name).ToListAsync();

        }
        return await _context.MenuItems.Where(i => i.CategoryId == categoryId && !(bool)i.IsDeleted!).OrderBy(i => i.Name).ToListAsync();
    }

    public async Task<(IEnumerable<MenuItem> list, int totalRecords )> GetPagedItemsAsync(int categoryId, string searchString, int page, int pagesize, bool isASC){

        IQueryable<MenuItem> query = _context.MenuItems.Where(i => i.CategoryId == categoryId && !(bool)i.IsDeleted!).OrderByDescending(i => i.ModifiedAt);

        if(!string.IsNullOrEmpty(searchString)){
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        if(isASC){ 
            query.OrderBy(i => i.Name);
        }
        else
        {
            query.OrderByDescending(i => i.Name);
        }

        return (await query.Skip((page-1)*pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());

    }


    public async Task<MenuItem?> GetByIdAsync(int id)
    {
        return await _context.MenuItems.SingleOrDefaultAsync(i => i.Id == id && !(bool)i.IsDeleted!);
    }

    public async Task AddAsync(MenuItem item)
    {
        _context.MenuItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MenuItem item)
    {
        _context.MenuItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<MenuItem> items)
    {
        _context.MenuItems.UpdateRange(items);
        await _context.SaveChangesAsync();
    }
    public async Task AddRangeAsync(IEnumerable<MappingMenuItemWithModifier> items)
    {
        _context.MappingMenuItemWithModifiers.AddRange(items);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateRangeOfMapping(IEnumerable<MappingMenuItemWithModifier> items)
    {
        _context.MappingMenuItemWithModifiers.UpdateRange(items);
        await _context.SaveChangesAsync();
    }
    public async Task RemoveRangeAsync(IEnumerable<MappingMenuItemWithModifier> items)
    {
        _context.MappingMenuItemWithModifiers.RemoveRange(items);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<MappingMenuItemWithModifier>> GetByItemId(int Id){

        return await _context.MappingMenuItemWithModifiers.Where(i => i.MenuItemId == Id).ToListAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<MenuItem, bool>> predicate)
    {
        return await _context.MenuItems.AnyAsync(predicate);
    }

     public async Task<IEnumerable<MenuItem>> GetMenuItemByCategory(int categoryId, string searchString){

        IQueryable<MenuItem> query = _context.MenuItems.Where(i => !(bool)i.IsDeleted!).OrderByDescending(i => i.ModifiedAt);

        if(!string.IsNullOrEmpty(searchString)){
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        if (categoryId != 0)
        {
            query = query.Where(i => i.CategoryId == categoryId);
        }
        
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<MenuItem>> GetAvailableByCategoryIdAsync(int categoryId)
    {
        return await _context.MenuItems.Where(i => i.CategoryId == categoryId && (bool)i.IsAvailable).ToListAsync();
    }
    public async Task<IEnumerable<MenuItem>> GetAllAvailableAsync()
    {
        return await _context.MenuItems.Where(i => (bool)i.IsAvailable).ToListAsync();
    }
    public async Task<IEnumerable<MenuItem>> GetFavoriteItemsAsync()
    {
        return await _context.MenuItems.Where(i => (bool)i.IsFavourite).ToListAsync();
    }

}
