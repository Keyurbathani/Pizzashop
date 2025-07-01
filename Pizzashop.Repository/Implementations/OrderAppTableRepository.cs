using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class OrderAppTableRepository : IOrderAppTableRepository
{
    private readonly PizzaShopContext _context;

    public OrderAppTableRepository(PizzaShopContext context)
    {
        _context = context;
    }

     public async Task<IEnumerable<Table>> GetTablesBySectionId(int sectionId)
    {
        return await _context.Tables.Where(i => i.SectionId == sectionId && !(bool)i.IsDeleted!).Include(i => i.Section).Include(i => i.TableOrderMappings).ThenInclude(i => i.Order).ToListAsync();
    }

    public async Task<IEnumerable<TableOrderMapping>> GetTableOrderMappings(int sectionId)
    {

        IQueryable<TableOrderMapping> query = _context.TableOrderMappings
        .Include(i => i.Table).ThenInclude(i => i.Section)
       .Include(i => i.Order);

        return await query.ToListAsync();

    }
   


}
