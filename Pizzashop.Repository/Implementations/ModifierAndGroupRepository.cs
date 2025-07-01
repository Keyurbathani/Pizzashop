using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class ModifierAndGroupRepository : IModifierAndGroupRepository
{
    private readonly PizzaShopContext _context;

    public ModifierAndGroupRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Modifier>> GetModifiersByGroupIdAsync(int modifierGroupId)
    {
        return await _context.ModifierAndGroups.Where(mag => mag.ModifiergroupId == modifierGroupId).Include(mag => mag.Modifier).ThenInclude(m => m.Unit).Select(mag => mag.Modifier).ToListAsync();
    }

    public async Task<IEnumerable<ModifierGroup>> GetModifierGroupsByModifierIdAsync(int modifierId)
    {
        return await _context.ModifierAndGroups.Where(mag => mag.ModifierId == modifierId).Include(mag => mag.Modifiergroup).Select(mag => mag.Modifiergroup).ToListAsync();
    }

    public async Task<IEnumerable<ModifierAndGroup>> GetModifiersAndGroupByGroupIdAsync(int modifierGroupId)
    {
        return await _context.ModifierAndGroups.Where(mag => mag.ModifiergroupId == modifierGroupId).ToListAsync();
    }

    public async Task<IEnumerable<ModifierAndGroup>> GetModifiersAndGroupByModifierIdAsync(int modifierId)
    {
        return await _context.ModifierAndGroups.Where(mag => mag.ModifierId == modifierId).ToListAsync();
    }

    public async Task<(IEnumerable<Modifier> list, int totalRecords)> GetPagedModifiersAsync(int modifierGroupId, string searchString, int page, int pagesize, bool isASC)
    {
        IQueryable<Modifier> query = _context.ModifierAndGroups.Include(m => m.Modifier).ThenInclude(m => m.Unit).Where(m => m.ModifiergroupId == modifierGroupId).Select(m => m.Modifier);
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(searchString));
        }

        if (isASC)
        {
            query.OrderBy(m => m.Name);
        }
        else
        {
            query.OrderByDescending(m => m.Name);
        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());

    }
    
    public async Task AddRangeAsync(IEnumerable<ModifierAndGroup> modifiersAndGroups)
    {
        await _context.ModifierAndGroups.AddRangeAsync(modifiersAndGroups);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<ModifierAndGroup> modifiersAndGroups)
    {
        _context.ModifierAndGroups.UpdateRange(modifiersAndGroups);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<ModifierAndGroup> modifiersAndGroups)
    {
        _context.ModifierAndGroups.RemoveRange(modifiersAndGroups);
        await _context.SaveChangesAsync();
    }

}
