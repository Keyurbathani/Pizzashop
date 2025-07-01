using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class OrderedItemRepository : IOrderedItemRepository
{
    private readonly PizzaShopContext _context;

    public OrderedItemRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<OrderedItem?> GetById(int id)
    {
        return await _context.OrderedItems.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<OrderedItem>> GetAllByOrderIdAsync(int orderId)
    {
        IQueryable<OrderedItem> query = _context.OrderedItems.Where(i => i.OrderId == orderId);
        return await query.ToListAsync();
    }

    public async Task AddItemsDetails(OrderedItem item)
    {
        _context.OrderedItems.Add(item);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateItemsDetails(OrderedItem item)
    {
        _context.OrderedItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<OrderedItem> item)
    {
        _context.OrderedItems.RemoveRange(item);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateRangeAsync(List<OrderedItem> item)
    {
        _context.OrderedItems.UpdateRange(item);
        await _context.SaveChangesAsync();
    }
    
    public async Task<int> GetReadyQuantityAsync(int id)
    {
        return await _context.OrderedItems.Where(i => i.Id == id).Select(i => i.ReadyItemQuantity.GetValueOrDefault()).SingleOrDefaultAsync();
    }

    public async Task<(IEnumerable<ItemSummaryDTO> mostSellingItems, IEnumerable<ItemSummaryDTO> leastSellingItems)> GetItemsSummary(DateOnly? fromDate, DateOnly? toDate){
        IQueryable<OrderedItem> query = _context.OrderedItems;
 
        if(fromDate != null){
            query = query.Where(oi => DateOnly.FromDateTime(oi.CreatedAt) >= fromDate);
        }
 
        if(toDate != null){
            query = query.Where(oi => DateOnly.FromDateTime(oi.CreatedAt) <= toDate);
        }
 
        IQueryable<ItemSummaryDTO>  queryItems = query.GroupBy(oi => oi.MenuItem).OrderByDescending(oi => oi.Count()).Select(i => new ItemSummaryDTO(){
            Id = i.Key.Id,
            Name = i.Key.Name,
            Image = i.Key.ProfileImage,
            OrderCount = i.Count()
        });
 
        List<ItemSummaryDTO> mostSellingItems = await queryItems.Take(5).ToListAsync();
        List<ItemSummaryDTO> leastSellingItems = await queryItems.Reverse().Take(5).ToListAsync();
 
        return (mostSellingItems, leastSellingItems);
    }



}
