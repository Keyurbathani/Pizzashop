using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ITableOrderMappingRepository
{
    Task AddMapping(TableOrderMapping tableOrder);
    Task UpdateMapping(TableOrderMapping tableOrder);

    Task<TableOrderMapping> GetTableOrderMapping(int orderId);
    Task<TableOrderMapping?> GetTableOrderMappingByTableId(int tableId);
    Task<IEnumerable<TableOrderMapping>> GetByOrderIdAsync(int orderId);
}
