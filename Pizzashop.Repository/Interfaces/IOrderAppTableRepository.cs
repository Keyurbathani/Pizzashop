using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IOrderAppTableRepository
{
    Task<IEnumerable<TableOrderMapping>> GetTableOrderMappings(int sectionId);

    Task<IEnumerable<Table>> GetTablesBySectionId(int sectionId);
}
