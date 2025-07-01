using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class OrderTaxMappingRepository : IOrderTaxMappingRepository
{
    private readonly PizzaShopContext _context;

    public OrderTaxMappingRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderTaxMapping>> GetAllByOrderId(int orderId)
    {
        return await _context.OrderTaxMappings.Where(i => i.OrderId == orderId).ToListAsync();
    }

    public async Task AddRangeAsync(List<OrderTaxMapping> item)
    {
        _context.OrderTaxMappings.AddRange(item);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateRangeAsync(List<OrderTaxMapping> item)
    {
        _context.OrderTaxMappings.UpdateRange(item);
        await _context.SaveChangesAsync();
    }


}
