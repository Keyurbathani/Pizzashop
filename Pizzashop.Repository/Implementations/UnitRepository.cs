using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class UnitRepository : IUnitRepository
{
    private readonly PizzaShopContext _context;

    public UnitRepository(PizzaShopContext context){
        _context =context;
    }

    public async Task<IEnumerable<Unit>> GetAllAsync(){
        return await _context.Units.OrderBy(c => c.Name).ToListAsync();
    }
}
