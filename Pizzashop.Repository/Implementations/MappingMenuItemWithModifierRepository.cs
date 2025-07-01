using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class MappingMenuItemWithModifierRepository : IMappingMenuItemWithModifierRepository
{
    private readonly PizzaShopContext _context;

    public MappingMenuItemWithModifierRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MappingMenuItemWithModifier>> GetModifierForItem(int itemId)
    {
        IQueryable<MappingMenuItemWithModifier> query = _context.MappingMenuItemWithModifiers.Where(i => i.MenuItemId == itemId)
        .Include(i => i.MenuItem)
        .Include(i => i.ModifierGroup)
        .ThenInclude(i => i.ModifierAndGroups)
        .ThenInclude(i => i.Modifier);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<MappingMenuItemWithModifier>> GetApplicableModifierByItemId(int itemId)
    {
        return await _context.MappingMenuItemWithModifiers.Where(im => im.MenuItemId == itemId).Include(im => im.ModifierGroup.ModifierAndGroups.Where(mg => !(bool)mg.IsDeleted && !(bool)mg.Modifier.IsDeleted)).ThenInclude(mg => mg.Modifier).ToListAsync();
    }
}
