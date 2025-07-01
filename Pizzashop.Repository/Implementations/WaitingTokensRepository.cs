using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class WaitingTokensRepository : IWaitingTokensRepository
{
    private readonly PizzaShopContext _context;

    public WaitingTokensRepository(PizzaShopContext context)
    {
        _context = context;
    }




    public async Task<IEnumerable<WaitingToken>> GetAllTokensBySection(int sectionId)
    {
        IQueryable<WaitingToken> query = _context.WaitingTokens.Where(i => !(bool)i.IsAssigned! && !(bool)i.IsDeleted!).Include(i => i.Customer).OrderByDescending(i => i.CustomerId);

        if (sectionId != 0)
        {
            query = query.Where(i => i.SectionId == sectionId);
        }

        return await query.ToListAsync();
    }
   
    public async Task<WaitingToken?> GetByIdAsync(int id)
    {
        return await _context.WaitingTokens.Include(i => i.Customer).SingleOrDefaultAsync(i => i.Id == id && !(bool)i.IsDeleted!);
    }
    public async Task<WaitingToken?> GetByCustomerId(int customerId)
    {
        return await _context.WaitingTokens.SingleOrDefaultAsync(i => i.CustomerId == customerId);
    }

    public async Task AddToken(WaitingToken token)
    {
        _context.WaitingTokens.Add(token);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateToken(WaitingToken token)
    {
        _context.WaitingTokens.Update(token);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteToken(WaitingToken token)
    {

        _context.WaitingTokens.Remove(token);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetWaitingCustomerCountAsync()
    {
        return await _context.WaitingTokens.Where(cv => !(bool)cv.IsAssigned).CountAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<WaitingToken, bool>> predicate)
    {
        return await _context.WaitingTokens.AnyAsync(predicate);
    }

}
