using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class SectionRepository : ISectionRepository
{
    private readonly PizzaShopContext _context;

    public SectionRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Section>> GetAllAsync()
    {
        return await _context.Sections.Where(c => !(bool)c.IsDeleted!).Include(i => i.WaitingTokens).OrderBy(c => c.Id).ToListAsync();
    }

    public async Task<Section?> GetByIdAsync(int id)
    {
        return await _context.Sections.SingleOrDefaultAsync(c => c.Id == id && !(bool)c.IsDeleted!);
    }

    public async Task AddAsync(Section section)
    {
        _context.Sections.Add(section);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Section section)
    {
        _context.Sections.Update(section);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<Section, bool>> predicate)
    {
        return await _context.Sections.AnyAsync(predicate);
    }

    public async Task<IEnumerable<Section>> GetAllSectionForApp()
    {
        IQueryable<Section> query =  _context.Sections.Where(i => !(bool)i.IsDeleted!)
                                    .Include(i => i.Tables)
                                    .ThenInclude(i => i.TableOrderMappings)
                                    .ThenInclude(i => i.Order);

        return await query.ToListAsync();
    }
   
}
