using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class TaxRepository : ITaxRepository
{

    private readonly PizzaShopContext _context;

    public TaxRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<TaxAndFee> list, int totalRecords)> GetPagedItemsAsync(string searchString, int page, int pagesize, bool isASC)
    {

        IQueryable<TaxAndFee> query = _context.TaxAndFees.Where(i => !(bool)i.IsDeleted!).OrderByDescending(i => i.ModifiedAt);

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        if (isASC)
        {
            query.OrderBy(i => i.Name);
        }
        else
        {
            query.OrderByDescending(i => i.Name);
        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());

    }
    
     public async Task<TaxAndFee?> GetByIdAsync(int id)
    {
        return await _context.TaxAndFees.SingleOrDefaultAsync(i => i.Id == id && !(bool)i.IsDeleted!);
    }

    public async Task AddAsync(TaxAndFee tax)
    {
        _context.TaxAndFees.Add(tax);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaxAndFee tax)
    {
        _context.TaxAndFees.Update(tax);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<TaxAndFee, bool>> predicate)
    {
        return await _context.TaxAndFees.AnyAsync(predicate);
    }
}
