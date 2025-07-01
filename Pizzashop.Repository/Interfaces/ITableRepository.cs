
using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ITableRepository
{
    Task<IEnumerable<Table>> GetTablesBySectionId(int sectionId);

    Task<(IEnumerable<Table> list, int totalRecords)> GetPagedItemsAsync(int SectionId, string searchString, int page, int pagesize, bool isASC);

    Task<Table?> GetByIdAsync(int id);

    Task AddAsync(Table table);

    Task UpdateAsync(Table table);

    Task UpdateRangeAsync(IEnumerable<Table> tables);

    Task<bool> IsExistsAsync(Expression<Func<Table, bool>> predicate);
    Task<IEnumerable<Table>> GetTableBySectionIdForAssign(int sectionId);
    int GetTotalCapacityBySectionId(int sectionId);


}
