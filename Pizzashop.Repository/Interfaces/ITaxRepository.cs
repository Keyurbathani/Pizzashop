using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ITaxRepository
{
    Task<(IEnumerable<TaxAndFee> list, int totalRecords)> GetPagedItemsAsync(string searchString, int page, int pagesize, bool isASC);
    Task<TaxAndFee?> GetByIdAsync(int id);

    Task AddAsync(TaxAndFee tax);

    Task UpdateAsync(TaxAndFee tax);

    Task<bool> IsExistsAsync(Expression<Func<TaxAndFee, bool>> predicate);
}
