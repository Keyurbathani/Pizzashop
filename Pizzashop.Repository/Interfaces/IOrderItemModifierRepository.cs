using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IOrderItemModifierRepository
{
    Task AddMapping(OrderedItemModifierMapping modifierMapping);
    Task<IEnumerable<OrderedItemModifierMapping>> GetAllByOrderItemIdAsync(int orderItemId);
    Task RemoveRangeAsync(List<OrderedItemModifierMapping> item);
    Task UpdateRangeAsync(List<OrderedItemModifierMapping> item);
    Task AddRangeAsync(List<OrderedItemModifierMapping> item);
}
