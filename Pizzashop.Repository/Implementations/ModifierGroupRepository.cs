using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class ModifierGroupRepository : IModifierGroupRepository
{
    private readonly PizzaShopContext _context;

    public ModifierGroupRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ModifierGroup>> GetAllAsync()
    {
        return await  _context.ModifierGroups.Where(c => !(bool)c.IsDeleted!).OrderBy(mg => mg.Id).ToListAsync();
    }

    public async Task<ModifierGroup?> GetByIdAsync(int id)
    {
        return await _context.ModifierGroups.SingleOrDefaultAsync(mg => mg.Id == id && !(bool)mg.IsDeleted!);
    }

    public async Task AddAsync(ModifierGroup modifierGroup)
    {
        _context.ModifierGroups.Add(modifierGroup);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ModifierGroup modifierGroup)
    {
        _context.ModifierGroups.Update(modifierGroup);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<ModifierGroup, bool>> predicate)
    {
        return await _context.ModifierGroups.AnyAsync(predicate);
    }


}
