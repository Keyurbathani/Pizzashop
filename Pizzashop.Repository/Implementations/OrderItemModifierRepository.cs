using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class OrderItemModifierRepository : IOrderItemModifierRepository
{
    private readonly PizzaShopContext _context;

    public OrderItemModifierRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task AddMapping(OrderedItemModifierMapping modifierMapping)
    {
        _context.OrderedItemModifierMappings.Add(modifierMapping);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderedItemModifierMapping>> GetAllByOrderItemIdAsync(int orderItemId)
    {
        return await _context.OrderedItemModifierMappings.Where(i => i.OrderItemId == orderItemId).ToListAsync();
    }
    public async Task RemoveRangeAsync(List<OrderedItemModifierMapping> item)
    {
        _context.OrderedItemModifierMappings.RemoveRange(item);
        await _context.SaveChangesAsync();
    }
    public async Task AddRangeAsync(List<OrderedItemModifierMapping> item)
    {
        _context.OrderedItemModifierMappings.AddRange(item);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateRangeAsync(List<OrderedItemModifierMapping> item)
    {
        _context.OrderedItemModifierMappings.UpdateRange(item);
        await _context.SaveChangesAsync();
    }
}
