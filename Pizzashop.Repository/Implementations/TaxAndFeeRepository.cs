using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class TaxAndFeeRepository : ITaxAndFeeRepository
{
     private readonly PizzaShopContext _context;

    public TaxAndFeeRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<TaxAndFee?> GetByIdAsync(int id)
    {
        return await _context.TaxAndFees.FirstOrDefaultAsync(i => i.Id == id);
    }
    public async Task<IEnumerable<TaxAndFee>> GetEnabledAsync()
    {
        return await _context.TaxAndFees.Where(i => (bool)i.IsActive).ToListAsync();
    }

}
