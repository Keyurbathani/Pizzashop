using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class ModifierRepository : IModifierRepository
{
    private readonly PizzaShopContext _context;

    public ModifierRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<Modifier?> GetByIdAsync(int id)
    {
        return await _context.Modifiers.SingleOrDefaultAsync(m => m.Id == id && !(bool)m.IsDeleted!);
    }

    public async Task<IEnumerable<Modifier>> GetAllAsync()
    {
        return await _context.Modifiers.Where(m => !(bool)m.IsDeleted!).Include(m => m.Unit).ToListAsync();
    }

    public async Task<(IEnumerable<Modifier> list, int totalRecords)> GetPagedExistingModifiers(string searchStringFor, int page, int pagesize)
    {
        IQueryable<Modifier> query = _context.Modifiers.Include(m => m.Unit).Where(m => !(bool)m.IsDeleted!);
        
        if (!string.IsNullOrEmpty(searchStringFor))
        {
            searchStringFor = searchStringFor.ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(searchStringFor));
        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());
    }



    public async Task AddAsync(Modifier modifier)
    {
        await _context.Modifiers.AddAsync(modifier);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Modifier modifier)
    {
        _context.Modifiers.Update(modifier);
        await _context.SaveChangesAsync();
    }

}
