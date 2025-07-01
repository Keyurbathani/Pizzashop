using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class TableOrderMappingRepository : ITableOrderMappingRepository
{
     private readonly PizzaShopContext _context;

    public TableOrderMappingRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public  async Task AddMapping(TableOrderMapping tableOrder)
    {

        _context.TableOrderMappings.Add(tableOrder);
        await _context.SaveChangesAsync();
    }
    
    public  async Task UpdateMapping(TableOrderMapping tableOrder)
    {
        _context.TableOrderMappings.Update(tableOrder);
        await _context.SaveChangesAsync();
    }

    public async Task<TableOrderMapping> GetTableOrderMapping(int orderId)
    {
        return await _context.TableOrderMappings.Where(i => i.OrderId == orderId).FirstOrDefaultAsync();
    }
    public async Task<TableOrderMapping?> GetTableOrderMappingByTableId(int tableId)
    {
        return await _context.TableOrderMappings.Include(i => i.Order).Where(i => i.TableId == tableId).FirstOrDefaultAsync();
    }

     public async Task<IEnumerable<TableOrderMapping>> GetByOrderIdAsync(int orderId)
    {

        IQueryable<TableOrderMapping> query = _context.TableOrderMappings.Where(i => i.OrderId == orderId);

        return await query.ToListAsync();

    }
}
