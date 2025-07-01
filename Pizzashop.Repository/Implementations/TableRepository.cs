using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class TableRepository : ITableRepository
{
     private readonly PizzaShopContext _context;

    public TableRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Table>> GetTablesBySectionId(int sectionId)
    {
        return await _context.Tables.Where(i => i.SectionId == sectionId && !(bool)i.IsDeleted!).ToListAsync();
    }

    public async Task<(IEnumerable<Table> list, int totalRecords )> GetPagedItemsAsync(int SectionId, string searchString, int page, int pagesize, bool isASC){

        IQueryable<Table> query = _context.Tables.Where(i => i.SectionId == SectionId && !(bool)i.IsDeleted!);

        if(!string.IsNullOrEmpty(searchString)){
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        if(isASC){ 
            query.OrderBy(i => i.Name);
        }
        else
        {
            query.OrderByDescending(i => i.Name);
        }

        return (await query.Skip((page-1)*pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());

    }

    public async Task<IEnumerable<Table>> GetTableBySectionIdForAssign(int sectionId)
    {
        IQueryable<Table> query = _context.Tables.Where(i => !(bool)i.IsDeleted! && (bool)i.IsAvailable);

        if (sectionId != 0)
        {
            query = query.Where(i => i.SectionId == sectionId);
        }

        return await query.ToListAsync();

    }

     public async Task<Table?> GetByIdAsync(int id)
    {
        return await _context.Tables.SingleOrDefaultAsync(i => i.Id == id && !(bool)i.IsDeleted!);
    }

    public async Task AddAsync(Table table)
    {
        _context.Tables.Add(table);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Table table)
    {
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Table> tables)
    {
        _context.Tables.UpdateRange(tables);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<Table, bool>> predicate)
    {
        return await _context.Tables.AnyAsync(predicate);
    }
     public  int GetTotalCapacityBySectionId(int sectionId)
    {
       return  _context.Tables.Where(i => i.SectionId == sectionId && !(bool)i.IsDeleted).Sum(i => i.Capacity.GetValueOrDefault());
    }
}
