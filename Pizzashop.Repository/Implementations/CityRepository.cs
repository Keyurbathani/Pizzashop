using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class CityRepository : ICityRepository
{
     private readonly PizzaShopContext _context;

    public CityRepository(PizzaShopContext context)
    {
        _context = context;
    }

     public async Task<City> GetCityById(int? cityId)
    {
        return await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
    }

    public async Task<List<City>> GetCitiesByStateId(int? stateId)
    {
        return await _context.Cities.Where(c => c.StateId == stateId).OrderBy(c => c.Name).ToListAsync();
    }
}

