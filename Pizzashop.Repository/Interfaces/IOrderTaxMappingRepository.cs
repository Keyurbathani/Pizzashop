using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IOrderTaxMappingRepository
{
    Task<IEnumerable<OrderTaxMapping>> GetAllByOrderId(int orderId);

    Task UpdateRangeAsync(List<OrderTaxMapping> item);

    Task AddRangeAsync(List<OrderTaxMapping> item);
}
