using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly PizzaShopContext _context;

    public CategoryRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MenuCategory>> GetAllAsync()
    {
        return await _context.MenuCategories.Where(c => !(bool)c.IsDeleted!).OrderBy(c => c.Id).ToListAsync();
    }

    public async Task<MenuCategory?> GetByIdAsync(int id)
    {
        return await _context.MenuCategories.SingleOrDefaultAsync(c => c.Id == id && !(bool)c.IsDeleted!);
    }

    public async Task AddAsync(MenuCategory category)
    {
        _context.MenuCategories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MenuCategory category)
    {
        _context.MenuCategories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<MenuCategory, bool>> predicate)
    {
        return await _context.MenuCategories.AnyAsync(predicate);
    }

}
