using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;

namespace Pizzashop.Repository.Interfaces;

public interface IOrderedItemRepository
{
    Task AddItemsDetails(OrderedItem item);
    Task UpdateItemsDetails(OrderedItem item);
    Task<OrderedItem?> GetById(int id);
    Task<IEnumerable<OrderedItem>> GetAllByOrderIdAsync(int orderId);
     Task RemoveRangeAsync(List<OrderedItem> item);
     Task UpdateRangeAsync(List<OrderedItem> item);
     Task<int> GetReadyQuantityAsync(int id);
     Task<(IEnumerable<ItemSummaryDTO> mostSellingItems, IEnumerable<ItemSummaryDTO> leastSellingItems)> GetItemsSummary(DateOnly? fromDate, DateOnly? toDate);
}
